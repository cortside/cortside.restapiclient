using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public interface IRestApiClient {
        IAuthenticator Authenticator { get; }
        IDistributedCache Cache { get; }
        IAsyncPolicy<RestResponse> Policy { get; }
        IRestSerializer Serializer { get; }
        Uri BuildUri(RestApiRequest request);
        Task<RestResponse<T>> ExecuteAndFollowAsync<T>(RestApiRequest request);
        Task<RestResponse> ExecuteAsync(RestApiRequest request);
        Task<RestResponse<T>> ExecuteAsync<T>(RestApiRequest request);
        Task<RestResponse> GetAsync(RestApiRequest request);
        Task<RestResponse<T>> GetAsync<T>(RestApiRequest request) where T : new();
        Task<T> GetWithCacheAsync<T>(RestApiRequest request, TimeSpan? duration = null) where T : class, new();
        Task<T> GetWithCacheAsync<T>(RestApiRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new();
    }
}
