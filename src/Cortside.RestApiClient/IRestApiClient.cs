using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;

namespace Cortside.RestApiClient {
    public interface IRestApiClient {
        IDistributedCache Cache { get; }
        Uri BuildUri(IRestApiRequest request);
        Task<RestResponse> ExecuteAsync(IRestApiRequest request);
        Task<RestResponse<T>> ExecuteAsync<T>(IRestApiRequest request);
        Task<RestResponse> GetAsync(IRestApiRequest request);
        Task<RestResponse<T>> GetAsync<T>(IRestApiRequest request) where T : new();
        Task<T> GetWithCacheAsync<T>(IRestApiRequest request, TimeSpan? duration = null) where T : class, new();
        Task<T> GetWithCacheAsync<T>(IRestApiRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new();
    }
}
