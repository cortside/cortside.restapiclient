using System;
using System.Net;
using System.Threading.Tasks;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.CatalogApi {
    public class CatalogClient : IDisposable, ICatalogClient {
        private readonly RestApiClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="clientConfiguration"></param>
        /// <param name="context"></param>
        /// <param name="throwOnAnyError">added this to make variable testing easier</param>
        public CatalogClient(ILogger<CatalogClient> logger, CatalogClientConfiguration clientConfiguration, IHttpContextAccessor context, bool throwOnAnyError = false) {
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(clientConfiguration.ServiceUrl),
                FollowRedirects = true,
                Authenticator = new OpenIDConnectAuthenticator(context, clientConfiguration.Authentication).UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                        .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                        .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                    )
                    .UseLogger(logger),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
                ThrowOnAnyError = throwOnAnyError
            };
            client = new RestApiClient(logger, context, options);
        }

        public async Task<RestResponse<CatalogItem>> GetItemAsync(string sku) {
            var request = new RestApiRequest($"api/v1/items/{sku}", Method.Get);
            var response = await client.ExecuteAsync<CatalogItem>(request).ConfigureAwait(false);
            return response;
        }

        public async Task<RestResponse<CatalogItem>> CreateItemAsync(bool followRedirects) {
            var request = new RestApiRequest("api/v1/items", Method.Post) {
                FollowRedirects = followRedirects
            };
            var response = await client.ExecuteAsync<CatalogItem>(request).ConfigureAwait(false);
            return response;
        }

        public async Task<RestResponse> SearchItemsAsync(bool followRedirects) {
            var request = new RestApiRequest("/api/v1/items/search", Method.Post) {
                FollowRedirects = followRedirects,
            };
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            return response;
        }

        public async Task<RestResponse> TemporaryRedirect() {
            var request = new RestApiRequest("/api/v1/302", Method.Get) {
                FollowRedirects = true,
            };
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            return response;
        }

        public async Task<RestResponse> ModelMismatchAsync() {
            var request = new RestApiRequest("/api/v1/jsonmodelmismatch", Method.Get) {
                FollowRedirects = true,
            };
            var response = await client.ExecuteAsync<CatalogItem>(request).ConfigureAwait(false);
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
