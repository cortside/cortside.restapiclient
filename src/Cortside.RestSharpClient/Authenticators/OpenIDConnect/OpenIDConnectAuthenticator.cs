using System;
using System.Threading.Tasks;
using Polly;
using RestSharp;
using RestSharp.Authenticators;

namespace Cortside.RestSharpClient.Authenticators.OpenIDConnect {
    public class OpenIDConnectAuthenticator : AuthenticatorBase {
        private readonly TokenRequest tokenRequest;
        private IAsyncPolicy<RestResponse> policy = Policy.NoOpAsync<RestResponse>();

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

        public IAuthenticator WithPolicy(IAsyncPolicy<RestResponse> policy) {
            this.policy = policy;
            return this;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            var token = string.IsNullOrEmpty(Token) ? await GetTokenAsync() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        private async Task<string> GetTokenAsync() {
            var options = new RestClientOptions(tokenRequest.AuthorityUrl);

            using (var client = new RestClient(options)) {
                var request = new RestRequest("connect/token", Method.Post)
                    .AddParameter("grant_type", tokenRequest.GrantType)
                    .AddParameter("scope", tokenRequest.Scope)
                    .AddParameter("client_id", tokenRequest.ClientId)
                    .AddParameter("client_secret", tokenRequest.ClientSecret);

                var response = await policy.ExecuteAsync(async () => {
                    //logger.LogInformation($"attempt with timeout = {request.Timeout}");
                    var response = await client.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);
                    if (response == null || response.StatusCode == 0) {
                        throw new TimeoutException();
                    }
                    return response;
                }).ConfigureAwait(false);

                if (response.IsSuccessful) {
                    var rr = client.Deserialize<TokenResponse>(response);
                    return $"{rr!.Data.TokenType} {rr!.Data.AccessToken}";
                } else {
                    return null;
                }
            }
        }
    }
}
