using System.Text;
using System.Threading.Tasks;
using WireMock;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace Cortside.RestApiClient.Tests.ResponseProviders {
    public class AlternateFailureResponse : IResponseProvider {
        private static int _count = 0;
        public Task<(IResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings) {
            ResponseMessage response;
            if (_count % 2 == 0) {
                response = new ResponseMessage() { StatusCode = 500 };
                SetBody(response, @"{ ""msg"": ""Hello some error from wiremock!"" }");
            } else {
                response = new ResponseMessage() { StatusCode = 200 };
                SetBody(response, @"{ ""msg"": ""Hello from wiremock!"" }");
            }
            _count++;
            (IResponseMessage, IMapping) tuple = (response, mapping);
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
