using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;

namespace Cortside.RestSharpClient {
    public interface IRestApiClient {
        IDistributedCache Cache { get; }
        Uri BuildUri(RestApiRequest request);
        Task<RestResponse> ExecuteAsync(RestApiRequest request);
        Task<RestResponse<T>> ExecuteAsync<T>(RestApiRequest request);
        Task<RestResponse> GetAsync(RestApiRequest request);
        Task<RestResponse<T>> GetAsync<T>(RestApiRequest request) where T : new();
        Task<T> GetWithCacheAsync<T>(RestApiRequest request, TimeSpan? duration = null) where T : class, new();
        Task<T> GetWithCacheAsync<T>(RestApiRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new();
    }
}
