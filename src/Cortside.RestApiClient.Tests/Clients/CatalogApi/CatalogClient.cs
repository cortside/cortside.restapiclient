using System;
using System.Threading.Tasks;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.CatalogApi {
    public class CatalogClient : IDisposable {
        readonly RestApiClient client;

        public CatalogClient(ILogger<HttpStatusClient> logger, CatalogClientConfiguration userClientConfiguration) {
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(userClientConfiguration.ServiceUrl),
                FollowRedirects = true,
                Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
            client = new RestApiClient(logger, options);
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

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            client?.Dispose();
        }
    }
}
