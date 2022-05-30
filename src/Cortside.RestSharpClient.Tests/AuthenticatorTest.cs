using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cortside.RestSharpClient.Authenticators.OpenIDConnect;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class AuthenticatorTest {
        [Fact]
        public async Task ShouldAddAuthorizationHeaderAsync() {
            // arrange
            var authenticator = new OpenIDConnectAuthenticator("https://demo.duendesoftware.com", "client_credentials", "m2m", "secret", "api")
                .WithPolicy(PolicyBuilderExtensions.HandleHttpError()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                );

            var client = new RestClient("http://api.github.com");
            var request = new RestRequest("foo", Method.Get);

            // act
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.True(authorization.Trim().Length > 1);
        }
    }
}
