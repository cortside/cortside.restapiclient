using System;
using System.Linq;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public class RestApiClient : IDisposable, IRestApiClient {
        private readonly ILogger logger;
        private readonly RestClient client;
        private readonly RestApiClientOptions options;

        public RestApiClient(string baseUrl, ILogger logger) {
            this.logger = logger;

            options = new RestApiClientOptions(baseUrl);
            client = new RestClient(options.Options);
        }

        public RestApiClient(RestApiClientOptions options, ILogger logger) {
            this.logger = logger;
            this.options = options;

            client = new RestClient(options.Options);
            if (options.Authenticator != null) {
                client.UseAuthenticator(options.Authenticator);
            }
            client.UseSerializer(() => options.Serializer);
        }

        public IAuthenticator Authenticator {
            get { return options.Authenticator; }
        }

        public IRestSerializer Serializer {
            get { return options.Serializer; }
        }

        public IDistributedCache Cache {
            get { return options.Cache; }
        }

        public Uri BuildUri(RestApiRequest request) => client.BuildUri(request.RestRequest);

        private void TimeoutCheck(RestApiRequest request, RestResponse response) {
            if (response.StatusCode == 0) {
                LogError(request, response);
            }
        }

        public IAsyncPolicy<RestResponse> Policy {
            get { return options.Policy; }
        }

        protected async Task<RestResponse> InnerExecuteAsync(RestApiRequest request) {
            var policy = request.Policy ?? options.Policy;

            var correlationId = CorrelationContext.GetCorrelationId();
            request.AddHeader("Request-Id", correlationId);
            request.AddHeader("X-Correlation-Id", correlationId);

            int retry = 0;
            var response = await policy.ExecuteAsync(async () => {
                logger.LogInformation($"Attempt {retry++}");

                var response = await client.ExecuteAsync(request.RestRequest).ConfigureAwait(false);
                logger.LogInformation("Response {retry}: Status Code = {StatusCode} Data = {Content}", retry, response.StatusCode, response.Content);
                TimeoutCheck(request, response);
                return response;
            }).ConfigureAwait(false);

            logger.LogDebug("Response to {url} returned with status code {StatusCode} and content: {Content}", client.BuildUri(request.RestRequest), response.StatusCode, response.Content);

            // TODO: should check status code -- what else?
            if (request.FollowRedirects ?? options.FollowRedirects && response.Headers.Any(h => h.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase))) {
                var url = response.Headers.FirstOrDefault(h => h.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase)).Value.ToString();
                logger.LogInformation($"Following redirect to {url}");
                var redirectRequest = new RestApiRequest(url, Method.Get);
                var redirectResponse = await InnerExecuteAsync(redirectRequest).ConfigureAwait(false);
                return redirectResponse;
            }

            return response;
        }

        public Task<RestResponse> ExecuteAsync(RestApiRequest request) {
            return InnerExecuteAsync(request);
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestApiRequest request) {
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);
            return client.Deserialize<T>(response);
        }

        public async Task<RestResponse> GetAsync(RestApiRequest request) {
            request.Method = Method.Get;
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
            } else {
                LogError(request, response);
                return default;
            }
        }

        public async Task<RestResponse<T>> GetAsync<T>(RestApiRequest request) where T : new() {
            request.Method = Method.Get;
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return client.Deserialize<T>(response);
            } else {
                LogError(request, response);
                return default;
            }
        }

        public Task<T> GetWithCacheAsync<T>(RestApiRequest request, TimeSpan? duration = null) where T : class, new() {
            var cacheKey = $"RestRequest::{client.BuildUri(request.RestRequest)}::{request.RestRequest.QueryParameters()}";
            return GetWithCacheAsync<T>(request, cacheKey, duration);
        }

        public async Task<T> GetWithCacheAsync<T>(RestApiRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new() {
            duration ??= TimeSpan.FromMinutes(10);
            var cacheOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration };

            var item = await Cache.GetValueAsync<T>(cacheKey, options.Serializer).ConfigureAwait(false);
            if (item == null) {
                var response = await InnerExecuteAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var rr = client.Deserialize<T>(response);
                    await Cache.SetValueAsync(cacheKey, rr.Data, options.Serializer, cacheOptions).ConfigureAwait(false);
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

        private void LogError(RestApiRequest request, RestResponse response) {
            //Get the values of the parameters passed to the API
            string parameters = request.RestRequest.QueryParameters();

            //Set up the information message with the URL, 
            //the status code, and the parameters.
            string info = "Request to " + BuildUri(request) + " failed with status code "
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            client?.Dispose();
        }
    }
}
