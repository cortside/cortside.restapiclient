using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;

namespace Cortside.RestSharpClient.Tests.Clients {
    public class HttpStatusClient : IDisposable {
        readonly RestSharpClient client;
        private readonly ILogger<HttpStatusClient> logger;

        public HttpStatusClient(ILogger<HttpStatusClient> logger, string hostUrl) {
            this.logger = logger;
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            client = new RestSharpClient(hostUrl, logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public HttpStatusClient(ILogger<HttpStatusClient> logger, string hostUrl, IDistributedCache cache) {
            client = new RestSharpClient(hostUrl, logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public async Task<string> Get200Async() {
            var request = new RestRequest("200retry", Method.Get);
            request.Timeout = 1000;
            var policy = GetRetryPolicy();
            var response = await client.UsePolicy(policy).GetAsync(request).ConfigureAwait(false);

            return response?.Content;
        }

        static IAsyncPolicy<RestResponse> GetRetryPolicy() {
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
