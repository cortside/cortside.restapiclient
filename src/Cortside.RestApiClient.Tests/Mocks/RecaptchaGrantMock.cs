using Cortside.MockServer;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Builder;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class RecaptchaGrantMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            server.WireMockServer
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b =>
                            b?.Contains("grant_type=recaptcha") == true && b.Contains("client_id=foo"))
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "foo-recaptcha-token"
                        }))
                );
        }
    }
}
