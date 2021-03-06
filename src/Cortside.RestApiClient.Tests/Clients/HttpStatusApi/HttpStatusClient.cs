using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.HttpStatusApi {
    public class HttpStatusClient : IDisposable {
        readonly RestApiClient client;

        public HttpStatusClient(ILogger<HttpStatusClient> logger, string hostUrl) {
            client = new RestApiClient(logger, hostUrl);
        }

        public HttpStatusClient(ILogger<HttpStatusClient> logger, RestApiClientOptions options) {
            client = new RestApiClient(logger, options);
        }

        public async Task<string> Get200Async() {
            var request = new RestApiRequest("200retry", Method.Get) {
                Timeout = 1000,
                Policy = PolicyBuilderExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutException>()
                    .OrResult(x => x.StatusCode == 0 || x.StatusCode == System.Net.HttpStatusCode.Unauthorized || x.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 5))
            };

            var response = await client.GetAsync(request).ConfigureAwait(false);
            return response?.Content;
        }

        public async Task<string> GetTimeoutAsync() {
            var request = new RestApiRequest("api/v1/timeout", Method.Get) {
                Timeout = 1000
            };
            var response = await client.GetAsync(request).ConfigureAwait(false);
            return response?.Content;
        }
        public async Task<RestResponse> ExecuteTimeoutAsync() {
            var request = new RestApiRequest("api/v1/timeout", Method.Get) {
                Timeout = 1000
            };
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            return response;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            client?.Dispose();
        }
    }
}
