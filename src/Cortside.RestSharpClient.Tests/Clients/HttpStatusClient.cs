using System;
using System.Threading.Tasks;
using Cortside.RestSharpClient.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Cortside.RestSharpClient.Tests.Clients {
    public class HttpStatusClient : IDisposable {
        readonly RestApiClient client;

        public HttpStatusClient(ILogger<HttpStatusClient> logger, string hostUrl) {
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(hostUrl),
                ThrowOnAnyError = true
            };
            client = new RestApiClient(logger, options);
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
                    .OrResult(x => ((int)x.StatusCode) == 0 || x.StatusCode == System.Net.HttpStatusCode.Unauthorized || x.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 5))
            };

            var response = await client.GetAsync(request).ConfigureAwait(false);
            return response?.Content;
        }

        internal async Task<RestResponse<CatalogItem>> GetItemAsync(string sku) {
            var request = new RestApiRequest($"api/v1/items/{sku}", Method.Get);
            var response = await client.ExecuteAsync<CatalogItem>(request).ConfigureAwait(false);
            return response;
        }

        internal async Task<RestResponse<CatalogItem>> CreateItemAsync() {
            var request = new RestApiRequest("api/v1/items", Method.Post) {
                FollowRedirects = true
            };
            var response = await client.ExecuteAsync<CatalogItem>(request).ConfigureAwait(false);
            return response;
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
