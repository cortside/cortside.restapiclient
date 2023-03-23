using Cortside.MockServer;
using Cortside.MockServer.AccessControl.Models;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class DelegationGrantMock : IMockHttpServerBuilder {
        public void Configure(WireMockServer server) {
            server
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b => b?.Contains("delegation") == true && b?.Contains("foo") == true)
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "delegation-token"
                        }))
                );
        }
    }
}
