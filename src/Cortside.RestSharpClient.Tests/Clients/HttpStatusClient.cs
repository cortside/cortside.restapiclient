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

            // execute the policy
            var response = await policy.ExecuteAsync(async () => {
                logger.LogInformation($"attempt with timeout = {request.Timeout}");
                var response = await client.GetAsync(request).ConfigureAwait(false);
                request.Timeout += 2000;
                if (response == null || response.StatusCode == 0) {
                    throw new TimeoutException();
                }
                return response;
            }).ConfigureAwait(false);

            return response?.Content;
        }

        static IAsyncPolicy<RestResponse> GetRetryPolicy() {
            //return HttpPolicyExtensions
            //    .HandleTransientHttpError()
            //    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            //    .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            // define the policy using WaitAndRetry (try 3 times, waiting an increasing numer of seconds if exception is thrown)
            return Policy<RestResponse>
                .Handle<Exception>()
                //.WaitAndRetryAsync(new[]
                //    {
                //    TimeSpan.FromSeconds(1),
                //    TimeSpan.FromSeconds(2),
                //    TimeSpan.FromSeconds(3)
                //    }
                //);
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
