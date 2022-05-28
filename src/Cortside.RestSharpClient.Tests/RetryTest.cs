using System.Text;
using System.Threading.Tasks;
using Cortside.RestSharpClient.Tests.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class RetryTest {
        [Fact]
        public async Task ShouldRetry() {
            // arrange
            var _server = WireMockServer.Start();
            _server.Given(Request.Create().WithPath("/200retry").UsingGet()).RespondWith(new CustomResponse());
            string url = "http://localhost:" + _server.Ports[0];

            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), url);

            // act
            var repos = await client.Get200Async().ConfigureAwait(false);

            // assert
            Assert.NotEmpty(repos);
        }
    }

    public class CustomResponse : IResponseProvider {
        private static int _count = 0;
        public Task<(IResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(IRequestMessage requestMessage, WireMockServerSettings settings) {
            ResponseMessage response;
            if (_count % 2 == 0) {
                response = new ResponseMessage() { StatusCode = 500 };
                SetBody(response, @"{ ""msg"": ""Hello some error from wiremock!"" }");
            } else {
                response = new ResponseMessage() { StatusCode = 200 };
                SetBody(response, @"{ ""msg"": ""Hello from wiremock!"" }");
            }
            _count++;
            (IResponseMessage, IMapping) tuple = (response, null);
            return Task.FromResult(tuple);
        }

        private void SetBody(ResponseMessage response, string body) {
            response.BodyDestination = BodyDestinationFormat.SameAsSource;
            response.BodyData = new BodyData {
                Encoding = Encoding.UTF8,
                DetectedBodyType = BodyType.String,
                BodyAsString = body
            };
        }
    }
}
