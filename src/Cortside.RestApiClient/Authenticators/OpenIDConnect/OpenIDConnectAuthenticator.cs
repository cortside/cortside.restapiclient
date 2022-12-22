#pragma warning disable _MissingAsync // TAP methods must end with Async.

using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace Cortside.RestApiClient.Authenticators.OpenIDConnect {
    public class OpenIDConnectAuthenticator : AuthenticatorBase {
        private readonly TokenRequest tokenRequest;
        private IAsyncPolicy<RestResponse> policy = Policy.NoOpAsync<RestResponse>();
        private ILogger<OpenIDConnectAuthenticator> logger = new NullLogger<OpenIDConnectAuthenticator>();

        public OpenIDConnectAuthenticator(TokenRequest tokenRequest) : base("") {
            this.tokenRequest = tokenRequest;
        }

        public OpenIDConnectAuthenticator(string authorityUrl, string grantType, string clientId, string clientSecret, string scope) : base("") {
            tokenRequest = new TokenRequest {
                AuthorityUrl = authorityUrl,
                GrantType = grantType,
                Scope = scope,
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        public OpenIDConnectAuthenticator UsePolicy(IAsyncPolicy<RestResponse> policy) {
            this.policy = policy;
            return this;
        }

        public OpenIDConnectAuthenticator UseLogger(ILogger<OpenIDConnectAuthenticator> logger) {
            this.logger = logger;
            return this;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            var token = string.IsNullOrEmpty(Token) ? await GetTokenAsync().ConfigureAwait(false) : Token;
            if (string.IsNullOrWhiteSpace(token)) {
                logger.LogWarning("Authentication token is null or empty, authenticated requests will fail");
                token = string.Empty; //setting token to empty so the restsharp addheaders call won't throw object reference exception
            }

            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        private async Task<string> GetTokenAsync() {
            var options = new RestClientOptions(tokenRequest.AuthorityUrl);

            using (var client = new RestClient(options)) {
                client.UseNewtonsoftJson();

                var request = new RestRequest("connect/token", Method.Post)
                    .AddParameter("grant_type", tokenRequest.GrantType)
                    .AddParameter("scope", tokenRequest.Scope)
                    .AddParameter("client_id", tokenRequest.ClientId)
                    .AddParameter("client_secret", tokenRequest.ClientSecret);

                var correlationId = CorrelationContext.GetCorrelationId();
                request.AddHeader("Request-Id", correlationId);
                request.AddHeader("X-Correlation-Id", correlationId);

                logger.LogInformation($"Getting token for client_id {tokenRequest.ClientId} with scopes [{tokenRequest.Scope}]");
                var response = await policy.ExecuteAsync(async () => {
                    var response = await client.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);
                    logger.LogInformation($"Identity Server response code: {response.StatusCode}");
                    return response;
                }).ConfigureAwait(false);

                if (response.IsSuccessful) {
                    // TODO: handling of deserialization exception?
                    // https://github.com/restsharp/RestSharp/blob/5830af48cf85b8eaadf89d83fbc3bf46106f5873/src/RestSharp/Serializers/DeseralizationException.cs
                    var rr = client.Deserialize<TokenResponse>(response);
                    logger.LogDebug("Authentication successful");
                    return $"{rr!.Data.TokenType} {rr!.Data.AccessToken}";
                } else {
                    logger.LogError(response.ErrorException, $"Identity Server response code: {response.StatusCode} with error {response.ErrorMessage}");
                    return null;
                }
            }
        }
    }
}
