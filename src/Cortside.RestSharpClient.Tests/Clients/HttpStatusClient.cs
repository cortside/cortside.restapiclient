using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Cortside.RestSharpClient.Tests.Clients {
    public class HttpStatusClient : IDisposable {
        readonly RestSharpClient client;

        public HttpStatusClient(ILogger<HttpStatusClient> logger, string hostUrl) {
            client = new RestSharpClient(hostUrl, logger) {
                Serializer = new JsonNetSerializer()
            };
        }

        public async Task<string> Get200Async() {
            var request = new RestApiRequest("200retry", Method.Get);
            request.Timeout = 1000;
            request.Policy = GetRetryPolicy();

            var response = await client.GetAsync(request).ConfigureAwait(false);
            return response?.Content;
        }

        private static IAsyncPolicy<RestResponse> GetRetryPolicy() {
            return PolicyBuilderExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutException>()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 5));
        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
