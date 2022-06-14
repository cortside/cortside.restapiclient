using System.Threading.Tasks;
using Cortside.RestSharpClient.Tests.Clients.HttpStatusApi;
using Cortside.RestSharpClient.Tests.ResponseProviders;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.Server;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class RetryTest {
        [Fact]
        public async Task ShouldRetryAsync() {
            // arrange
            var _server = WireMockServer.Start();
            _server.Given(Request.Create().WithPath("/200retry").UsingGet()).RespondWith(new AlternateFailureResponse());
            string url = "http://localhost:" + _server.Ports[0];

            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), url);

            // act
            var repos = await client.Get200Async().ConfigureAwait(false);

            // assert
            Assert.NotEmpty(repos);
        }
    }
}
