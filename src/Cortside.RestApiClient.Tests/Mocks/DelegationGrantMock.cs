using Cortside.MockServer;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Builder;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class DelegationGrantMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            server.WireMockServer
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b => b?.Contains("grant_type=delegation") == true && b.Contains("client_id=foo") == true)
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "foo-delegation-token"
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b => b?.Contains("grant_type=client_credentials") == true && b.Contains("client_id=foo") == true)
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "foo-client_credentials-token"
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b => b?.Contains("grant_type=delegation") == true && b.Contains("client_id=allow"))
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "allow-delegation-token"
                        }))
                );

            server.WireMockServer
                .Given(
                    Request.Create()
                        .WithPath("/connect/token")
                        .WithBody(b => b?.Contains("grant_type=client_credentials") == true && b.Contains("client_id=allow") == true)
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                            TokenType = "Bearer",
                            ExpiresIn = "3600",
                            AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2OTA0ODIyNzEsImV4cCI6MTcyMjAxODI3MSwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsImdyYW50X3R5cGUiOiJkZWxlZ2F0aW9uIn0.x43mQWtHy_ubj36gMXuoYtqp6K32w9XVuooYXWWVQNo"
                        }))
                );
        }
    }
}
