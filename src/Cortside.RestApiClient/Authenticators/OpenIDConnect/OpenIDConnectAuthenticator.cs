#pragma warning disable _MissingAsync // TAP methods must end with Async.

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Microsoft.AspNetCore.Http;
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
        private ILogger logger = new NullLogger<OpenIDConnectAuthenticator>();
        private readonly DateTime tokenExpiration = DateTime.UtcNow;
        private readonly IHttpContextAccessor context;

        public OpenIDConnectAuthenticator(IHttpContextAccessor context, TokenRequest tokenRequest) : base("") {
            this.context = context;
            this.tokenRequest = tokenRequest;
        }

        public OpenIDConnectAuthenticator(IHttpContextAccessor context, string authorityUrl, string grantType, string clientId, string clientSecret, string scope) : base("") {
            this.context = context;
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

        public OpenIDConnectAuthenticator UseLogger(ILogger logger) {
            this.logger = logger;
            return this;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            // TODO: check for expired token
            var token = string.IsNullOrEmpty(Token) ? await GetTokenAsync().ConfigureAwait(false) : Token;
            if (string.IsNullOrWhiteSpace(token)) {
                logger.LogWarning("Authentication token is null or empty, authenticated requests will fail");
                token = string.Empty; //setting token to empty so the restsharp addheaders call won't throw object reference exception
            }

            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        public async Task<string> GetTokenAsync() {
            var response = await GetTokenAsync(tokenRequest.AuthorityUrl, tokenRequest.GrantType, tokenRequest.ClientId, tokenRequest.ClientSecret, tokenRequest.Scope).ConfigureAwait(false);
            if (!response.IsSuccessful) {
                return null;
            }

            var result = $"{response!.Data.TokenType} {response!.Data.AccessToken}";

            var allowsDelegation = AllowsDelegation(response?.Data?.AccessToken);
            if (allowsDelegation && context?.HttpContext != null) {
                var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(authorization)) {
                    authorization = authorization.Replace("Bearer ", "");
                    response = await GetTokenAsync(tokenRequest.AuthorityUrl, "delegation", tokenRequest.ClientId, tokenRequest.ClientSecret, tokenRequest.Scope, authorization).ConfigureAwait(false);
                    if (response.IsSuccessful) {
                        return $"{response!.Data.TokenType} {response!.Data.AccessToken}";
                    }

                    return null;
                }
            }

            return result;
        }

        private bool AllowsDelegation(string token) {
            if (string.IsNullOrWhiteSpace(token)) {
                return false;
            }

            try {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var delegation = jwtToken.Claims.Any(x => x.Type == "grant_type" && x.Value == "delegation");
                // check to see if token is for the client to be used for 
                //var self = jwtToken.Claims.Any(x => x.Type == "client_id" && x.Value == tokenRequest.ClientId);

                //return delegation && !self;
                return delegation;
            } catch (Exception ex) {
                logger.LogDebug(ex, "Unable to read token as JWT token to figure out if token has grant_type claim for delegation");
                return false;
            }
        }

        private async Task<RestResponse<TokenResponse>> GetTokenAsync(string url, string grantType, string clientId, string clientSecret, string scope, string token = null) {
            var options = new RestClientOptions(url);

            using (var client = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson())) {
                var request = new RestRequest("connect/token", Method.Post)
                    .AddParameter("grant_type", grantType)
                    .AddParameter("scope", scope)
                    .AddParameter("client_id", clientId)
                    .AddParameter("client_secret", clientSecret)
                    .AddParameter("token", token ?? string.Empty);

                var correlationId = CorrelationContext.GetCorrelationId();
                request.AddHeader("Request-Id", correlationId);
                request.AddHeader("X-Correlation-Id", correlationId);

                // TODO: add ip??

                logger.LogInformation($"Getting {grantType} token for client_id {clientId} with scopes [{scope}]");
                var response = await policy.ExecuteAsync(async () => {
                    var response = await client.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);
                    logger.LogInformation($"Identity Server response code: {response?.StatusCode}");
                    return response;
                }).ConfigureAwait(false);

                if (response.IsSuccessful) {
                    // TODO: handling of deserialization exception?
                    // https://github.com/restsharp/RestSharp/blob/5830af48cf85b8eaadf89d83fbc3bf46106f5873/src/RestSharp/Serializers/DeseralizationException.cs
                    var rr = client.Deserialize<TokenResponse>(response);
                    logger.LogDebug($"Successfully obtained {grantType} token for client_id {clientId} with scopes [{scope}]");

                    return rr;
                } else {
                    logger.LogError(response.ErrorException, $"Failed to obtain {grantType} token for client_id {clientId} with scopes [{scope}], Identity Server responded with status: {response.StatusCode} and error {response.ErrorMessage}");
                    return RestResponse<TokenResponse>.FromResponse(response);
                }
            }
        }
    }
}
