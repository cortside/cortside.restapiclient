using System.Threading.Tasks;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.ResponseProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.Server;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class RetryTest {
        [Fact]
        public async Task ShouldRetryAsync() {
            // arrange
            var _server = WireMockServer.Start();
            _server.Given(Request.Create().WithPath("/200retry").UsingGet()).RespondWith(new AlternateFailureResponse());
            string url = "http://localhost:" + _server.Ports[0];

            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), url, new HttpContextAccessor());

            // act
            var repos = await client.Get200Async();

            // assert
            Assert.NotEmpty(repos);
        }
    }
}
