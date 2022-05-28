using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public class RestSharpClient : IDisposable {
        private readonly ILogger logger;
        private readonly RestClient client;
        private IRestSerializer serializer;

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

        public async Task<RestResponse> ExecuteAsync(RestRequest request) {
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            TimeoutCheck(request, response);
            return response;
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) {
            var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
            TimeoutCheck(request, response);
            return response;
        }

        public async Task<RestResponse> GetAsync(RestRequest request) {
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
            } else {
                LogError(request, response);
                return default;
            }
        }

        public async Task<RestResponse<T>> GetAsync<T>(RestRequest request) where T : new() {
            var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
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
                var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    await Cache.SetValueAsync(cacheKey, response.Data, serializer, cacheOptions).ConfigureAwait(false);
                    item = response.Data;
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

        //private static int _maxRetryAttempts = 5;
        //private static TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(10);

        //private RestResponse RestResponseWithPolicy(RestClient restClient, RestRequest restRequest, Func<string, Task> logFunction) {
        //    var retryPolicy = Policy
        //        .HandleResult<RestResponse>(x => !x.IsSuccessful)
        //        .WaitAndRetry(_maxRetryAttempts, x => _pauseBetweenFailures, async (iRestResponse, timeSpan, retryCount, context) => {
        //            await logFunction($"The request failed. HttpStatusCode={iRestResponse.Result.StatusCode}. Waiting {timeSpan} seconds before retry. Number attempt {retryCount}. Uri={iRestResponse.Result.ResponseUri}; RequestResponse={iRestResponse.Result.Content}");
        //        });

        //    var circuitBreakerPolicy = Policy
        //        .HandleResult<RestResponse>(x => x.StatusCode == HttpStatusCode.ServiceUnavailable)
        //        .CircuitBreaker(1, TimeSpan.FromSeconds(60), onBreak: async (iRestResponse, timespan, context) => {
        //            await logFunction($"Circuit went into a fault state. Reason: {iRestResponse.Result.Content}");
        //        },
        //        onReset: async (context) => {
        //            await logFunction($"Circuit left the fault state.");
        //        });

        //    return retryPolicy.Wrap(circuitBreakerPolicy).Execute(() => restClient.Execute(restRequest));
        //}

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
