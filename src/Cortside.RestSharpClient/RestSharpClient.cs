using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public class RestSharpClient : IDisposable {
        private readonly ILogger logger;
        private readonly RestClient client;
        private IRestSerializer serializer;
        private IAsyncPolicy<RestResponse> policy = Policy.NoOpAsync<RestResponse>();

        public RestSharpClient(string baseUrl, ILogger logger) {
            Cache = new NullDistributedCache();
            this.logger = logger;

            var options = new RestClientOptions {
                BaseUrl = new Uri(baseUrl)
            };

            client = new RestClient(options);
            Serializer = new JsonNetSerializer();
        }

        public RestSharpClient(RestClientOptions options, ILogger logger) {
            Cache = new NullDistributedCache();
            this.logger = logger;

            client = new RestClient(options);
            Serializer = new JsonNetSerializer();
        }

        public IAuthenticator Authenticator {
            get {
                return client.Authenticator;
            }
            set {
                client.UseAuthenticator(value);
            }
        }

        public IRestSerializer Serializer {
            get {
                return serializer;
            }
            set {
                serializer = value;
                client.UseSerializer(() => value);
            }
        }

        public IDistributedCache Cache { get; set; }

        public Uri BuildUri(RestRequest request) => client.BuildUri(request);

        private void TimeoutCheck(RestRequest request, RestResponse response) {
            if (response.StatusCode == 0) {
                LogError(request, response);
            }
        }

        public RestSharpClient WithPolicy(IAsyncPolicy<RestResponse> policy) {
            this.policy = policy;
            return this;
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request) {
            var response = await policy.ExecuteAsync(async () => {
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                if (response == null || response.StatusCode == 0) {
                    throw new TimeoutException();
                }
                return response;
            }).ConfigureAwait(false);

            TimeoutCheck(request, response);
            return response;
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) {
            var response = await policy.ExecuteAsync(async () => {
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                if (response == null || response.StatusCode == 0) {
                    throw new TimeoutException();
                }
                return response;
            }).ConfigureAwait(false);

            TimeoutCheck(request, response);
            return client.Deserialize<T>(response);
        }

        public async Task<RestResponse> GetAsync(RestRequest request) {
            var response = await policy.ExecuteAsync(async () => {
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                if (response == null || response.StatusCode == 0) {
                    throw new TimeoutException();
                }
                return response;
            }).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
            } else {
                LogError(request, response);
                return default;
            }
        }

        public async Task<RestResponse<T>> GetAsync<T>(RestRequest request) where T : new() {
            var response = await policy.ExecuteAsync(async () => {
                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                if (response == null || response.StatusCode == 0) {
                    throw new TimeoutException();
                }
                return response;
            }).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return client.Deserialize<T>(response);
            } else {
                LogError(request, response);
                return default;
            }
        }

        public Task<T> GetWithCacheAsync<T>(RestRequest request) where T : class, new() {
            var cacheKey = $"RestRequest::{client.BuildUri(request)}::{request.QueryParameters()}";
            return GetWithCacheAsync<T>(request, cacheKey);
        }

        public async Task<T> GetWithCacheAsync<T>(RestRequest request, string cacheKey) where T : class, new() {
            var cacheOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

            var item = await Cache.GetValueAsync<T>(cacheKey, serializer).ConfigureAwait(false);
            if (item == null) {
                var response = await policy.ExecuteAsync(async () => {
                    var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                    if (response == null || response.StatusCode == 0) {
                        throw new TimeoutException();
                    }
                    return response;
                }).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var rr = client.Deserialize<T>(response);
                    await Cache.SetValueAsync(cacheKey, rr.Data, serializer, cacheOptions).ConfigureAwait(false);
                    item = rr.Data;
                } else {
                    LogError(request, response);
                    return default;
                }
            }

            return item;
        }

        private void LogError(RestRequest request, RestResponse response) {
            //Get the values of the parameters passed to the API
            string parameters = request.QueryParameters();

            //Set up the information message with the URL, 
            //the status code, and the parameters.
            string info = "Request to " + client.BuildUri(request) + " failed with status code "
                          + response.StatusCode + ", parameters: "
                          + parameters + ", and content: " + response.Content;

            //Acquire the actual exception
            Exception ex;
            if (response?.ErrorException != null) {
                ex = response.ErrorException;
            } else {
                ex = new Exception(info);
                info = string.Empty;
            }

            //Log the exception and info message
            logger.LogError(ex, info);
        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
