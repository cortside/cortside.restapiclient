using System;
using System.Threading.Tasks;
using Cortside.RestApiClient.Authenticators;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients {
    public class TestAuthenticator : RestApiAuthenticator {
        private DateTime expirationDate;

        public TestAuthenticator() : base(String.Empty) { }

        protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            if (string.IsNullOrEmpty(Token) || expirationDate >= DateTime.UtcNow) {
                Token = Guid.NewGuid().ToString();
                expirationDate = DateTime.UtcNow.AddDays(1);
            }

            return ValueTask.FromResult<Parameter>(new HeaderParameter(KnownHeaders.Authorization, Token));
        }

        public override void HandleUnauthorizedClientRequest() {
            // do other things here that might be needed outside of what base does
            expirationDate = DateTime.MinValue;

            base.HandleUnauthorizedClientRequest();
        }

        public string CachedToken => Token;
    }
}
