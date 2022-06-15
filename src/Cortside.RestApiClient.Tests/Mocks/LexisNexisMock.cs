using Cortside.MockServer;
using Cortside.RestApiClient.Tests.ResponseProviders;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Cortside.RestApiClient.Tests.Mocks {
    public class LexisNexisMock : IMockHttpServerBuilder {
        public void Configure(WireMockServer server) {
            // generate response depending on what tester requests in street1 field
            server
                .Given(
                Request.Create().UsingPost()
                    .WithPath("/WsIdentity/InstantID")
                    .WithHeader("Authorization", "Basic Zm9vOmJhcg==") //base64 encoding of `foo:bar`
                )
                .AtPriority(1000) // set low to allow overrides by json mappings
                .RespondWith(
                    Response.Create()
                        .WithBody(r => InstantId.BuildResponse(r.Body))
                        .WithHeader("Content-Type", "text/xml")
                );
        }
    }
}
