using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace Cortside.RestSharpClient.Authenticators.OpenIDConnect {
    public class OpenIDConnectAuthenticator : AuthenticatorBase {
        private readonly string authorityUrl;
        private readonly TokenRequest tokenRequest;

        public OpenIDConnectAuthenticator(string authorityUrl, string grantType, string clientId, string clientSecret, string scope) : base("") {
            this.authorityUrl = authorityUrl;
            tokenRequest = new TokenRequest {
                GrantType = grantType,
                Scope = scope,
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            var token = string.IsNullOrEmpty(Token) ? await GetTokenAsync() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        private async Task<string> GetTokenAsync() {
            var options = new RestClientOptions(authorityUrl);

            using (var client = new RestClient(options)) {
                var request = new RestRequest("connect/token", Method.Post)
                    .AddParameter("grant_type", tokenRequest.GrantType)
                    .AddParameter("scope", tokenRequest.Scope)
                    .AddParameter("client_id", tokenRequest.ClientId)
                    .AddParameter("client_secret", tokenRequest.ClientSecret);

                var response = await client.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);
                if (response.IsSuccessful) {
                    return $"{response!.Data.TokenType} {response!.Data.AccessToken}";
                } else {
                    return null;
                }
            }
        }
    }
}
