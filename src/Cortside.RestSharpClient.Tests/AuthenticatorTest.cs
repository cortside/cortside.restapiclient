using System.Linq;
using System.Threading.Tasks;
using Cortside.RestSharpClient.Authenticators.OpenIDConnect;
using RestSharp;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class AuthenticatorTest {
        [Fact]
        public async Task ShouldAddAuthorizationHeaderAsync() {
            // arrange
            var authenticator = new OpenIDConnectAuthenticator("https://demo.identityserver.io", "client_credentials", "m2m", "secret", "api");

            var client = new RestClient("http://api.github.com");
            var request = new RestRequest("foo", Method.Get);

            // act
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

            // assert
            Assert.Contains(request.Parameters, x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization);
            Assert.True(request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization).Value.ToString().Trim().Length > 1);
        }
    }
}
