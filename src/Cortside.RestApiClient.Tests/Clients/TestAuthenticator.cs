using System;
using System.Threading.Tasks;
using Cortside.RestApiClient.Authenticators;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients {
    public class TestAuthenticator : RestApiAuthenticator {
        public TestAuthenticator() : base(String.Empty) {
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            if (string.IsNullOrEmpty(Token)) {
                Token = Guid.NewGuid().ToString();
            }

            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        public override void HandleUnauthorizedClientRequest() {
            Token = null;
        }

        public string CachedToken => Token;
    }
}
