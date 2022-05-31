using System;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
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

        //TODO: use builder pattern
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

        public RestSharpClient UsePolicy(IAsyncPolicy<RestResponse> policy) {
            this.policy = policy;
            return this;
        }

        protected async Task<RestResponse> InnerExecuteAsync(RestRequest request) {
            var correlationId = CorrelationContext.GetCorrelationId();
            request.AddHeader("Request-Id", correlationId);
            request.AddHeader("X-Correlation-Id", correlationId);

            int retry = 0;
            var response = await policy.ExecuteAsync(async () => {
                logger.LogInformation($"Attempt {retry++}");

                var response = await client.ExecuteAsync(request).ConfigureAwait(false);
                logger.LogInformation("Response {retry}: Status Code = {StatusCode} Data = {Content}", retry, response.StatusCode, response.Content);
                TimeoutCheck(request, response);
                return response;
            }).ConfigureAwait(false);

            logger.LogDebug("Response to {url} returned with status code {StatusCode} and content: {Content}", client.BuildUri(request), response.StatusCode, response.Content);
            return response;
        }

        public Task<RestResponse> ExecuteAsync(RestRequest request) {
            return InnerExecuteAsync(request);
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) {
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);
            return client.Deserialize<T>(response);
        }

        public async Task<RestResponse> GetAsync(RestRequest request) {
            request.Method = Method.Get;
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
            } else {
                LogError(request, response);
                return default;
            }
        }

        public async Task<RestResponse<T>> GetAsync<T>(RestRequest request) where T : new() {
            request.Method = Method.Get;
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return client.Deserialize<T>(response);
            } else {
                LogError(request, response);
                return default;
            }
        }

        public Task<T> GetWithCacheAsync<T>(RestRequest request, TimeSpan? duration = null) where T : class, new() {
            var cacheKey = $"RestRequest::{client.BuildUri(request)}::{request.QueryParameters()}";
            return GetWithCacheAsync<T>(request, cacheKey, duration);
        }

        public async Task<T> GetWithCacheAsync<T>(RestRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new() {
            duration ??= TimeSpan.FromMinutes(10);
            var cacheOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration };

            var item = await Cache.GetValueAsync<T>(cacheKey, serializer).ConfigureAwait(false);
            if (item == null) {
                var response = await InnerExecuteAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var rr = client.Deserialize<T>(response);
                    await Cache.SetValueAsync(cacheKey, rr.Data, serializer, cacheOptions).ConfigureAwait(false);
                    item = rr.Data;
                } else {
                    LogError(request, response);
                    var ex = response.ErrorException;
                    if (ex != null) {
                        throw ex;
                    }
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
