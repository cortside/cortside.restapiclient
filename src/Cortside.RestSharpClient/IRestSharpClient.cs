using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public interface IRestSharpClient {
        IAuthenticator Authenticator { get; set; }
        IDistributedCache Cache { get; set; }
        IAsyncPolicy<RestResponse> Policy { get; set; }
        IRestSerializer Serializer { get; set; }
        Uri BuildUri(RestRequest request);
        Task<RestResponse<T>> ExecuteAndFollowAsync<T>(RestRequest request);
        Task<RestResponse> ExecuteAsync(RestRequest request);
        Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request);
        Task<RestResponse> GetAsync(RestRequest request);
        Task<RestResponse<T>> GetAsync<T>(RestRequest request) where T : new();
        Task<T> GetWithCacheAsync<T>(RestRequest request, TimeSpan? duration = null) where T : class, new();
        Task<T> GetWithCacheAsync<T>(RestRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new();
        RestSharpClient UsePolicy(IAsyncPolicy<RestResponse> policy);
    }
}
