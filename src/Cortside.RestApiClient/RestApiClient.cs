using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Xml;
using Serilog.Context;

namespace Cortside.RestApiClient {
    public class RestApiClient : IDisposable, IRestApiClient {
        private readonly ILogger logger;
        private readonly RestClient client;
        private readonly RestApiClientOptions options;
        private readonly IHttpContextAccessor contextAccessor;

        [Obsolete("Use overload that takes IHttpContextAccessor")]
        public RestApiClient(ILogger logger, string baseUrl) : this(logger, new HttpContextAccessor(), baseUrl) {
        }

        public RestApiClient(ILogger logger, IHttpContextAccessor contextAccessor, string baseUrl) : this(logger, contextAccessor, new RestApiClientOptions(baseUrl)) {
        }

        [Obsolete("Use overload that takes IHttpContextAccessor")]
        public RestApiClient(ILogger logger, RestApiClientOptions options) : this(logger, new HttpContextAccessor(), options) {
        }

        public RestApiClient(ILogger logger, IHttpContextAccessor contextAccessor, RestApiClientOptions options, HttpClient httpClient = null) {
            this.logger = logger;
            this.options = options;
            this.contextAccessor = contextAccessor;

            if (options.Serializer != null) {
                if (httpClient != null) {
                    client = new RestClient(httpClient, options.Options, configureSerialization: s => s.UseSerializer(() => options.Serializer));
                } else {
                    client = new RestClient(options.Options, configureSerialization: s => s.UseSerializer(() => options.Serializer));
                }
            }
            if (options.XmlSerializer) {
                if (httpClient != null) {
                    client = new RestClient(httpClient, options.Options, configureSerialization: s => s.UseXmlSerializer());
                } else {
                    client = new RestClient(options.Options, configureSerialization: s => s.UseXmlSerializer());
                }
            }
        }

        public IDistributedCache Cache {
            get { return options.Cache; }
        }

        public Uri BuildUri(IRestApiRequest request) => client.BuildUri(request.RestRequest);

        private void TimeoutCheck(IRestApiRequest request, RestResponse response) {
            if (response.ResponseStatus == ResponseStatus.TimedOut) {
                response.ErrorMessage = null;
                response.ErrorException = null;
                LogError(request, response);

                if (options.ThrowOnAnyError) {
                    var timeouts = new int[] { request.Timeout, client.Options.MaxTimeout };
                    var timeout = timeouts.Where(x => x > 0).Min();
                    throw new TimeoutException($"Timeout of {timeout} exceeded");
                }
            }
        }

        private RestResponse<T> DeserializeRestResponse<T>(IRestApiRequest request, RestResponse response) {
            var result = client.Deserialize<T>(response);

            var ex = result.ErrorException;
            if ((options.ThrowOnDeserializationError || options.ThrowOnAnyError) && ex != null) {
                LogError(request, result);
                throw ex;
            }

            return result;
        }

        protected async Task<RestResponse> InnerExecuteAsync(IRestApiRequest request) {
            var policy = request.Policy ?? options.Policy;

            var correlationId = CorrelationContext.GetCorrelationId();
            request.AddHeader("Request-Id", correlationId);
            request.AddHeader("X-Correlation-Id", correlationId);
            SetForwardedHeader(request);

            RestResponse response;
            using (LogContext.PushProperty("RequestUrl", client.BuildUri(request.RestRequest))) {
                int attempt = 0;
                try {
                    response = await policy.ExecuteAsync(async () => await InnerExecuteAttemptAsync(request, attempt++))
                        .ConfigureAwait(false);
                } catch (Exception ex) {
                    response = new RestResponse() {
                        ErrorException = ex,
                        ErrorMessage = ex.Message,
                        IsSuccessStatusCode = false,
                        StatusCode = 0
                    };
                }

                logger.LogInformation("Response from {url} returned with status code {StatusCode} and content: {Content}", client.BuildUri(request.RestRequest), response.StatusCode, response.Content);
                TimeoutCheck(request, response);
                var exception = response.ErrorException;
                if (options.ThrowOnAnyError && exception != null) {
                    throw exception;
                }

                if (response.StatusCode == HttpStatusCode.SeeOther && (request.FollowRedirects ?? options.FollowRedirects) && response.Headers?.Any(h => h.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase)) == true) {
                    var url = response.Headers.FirstOrDefault(h => h.Name.Equals("location", StringComparison.InvariantCultureIgnoreCase)).Value.ToString();
                    logger.LogInformation($"Following redirect to {url}");
                    var redirectRequest = new RestApiRequest(url, Method.Get);
                    var redirectResponse = await InnerExecuteAsync(redirectRequest).ConfigureAwait(false);
                    return redirectResponse;
                }
            }

            return response;
        }

        private async Task<RestResponse> InnerExecuteAttemptAsync(IRestApiRequest request, int attempt) {
            var url = client.BuildUri(request.RestRequest);
            var body = request.Parameters.SingleOrDefault(x => x.Type == ParameterType.RequestBody);
            var serializer = options.Serializer ?? new JsonNetSerializer();
            var json = serializer.Serialize(body);
            logger.LogDebug("Request to {url}, attempt {attempt}, with body {body}", url, attempt, json);

            var response = await client.ExecuteAsync(request.RestRequest).ConfigureAwait(false);
            if (response.ErrorException != null || !string.IsNullOrWhiteSpace(response.ErrorMessage)) {
                logger.LogError(response.ErrorException,
                    "Response {attempt}: Status Code = {StatusCode} ErrorMessage = {ErrorMessage} Content = {Content}", attempt,
                    response.StatusCode, response.ErrorMessage, response.Content);
            } else {
                logger.LogDebug("Response {attempt}: Status Code = {StatusCode} Content = {Content}", attempt, response.StatusCode,
                    response.Content);
            }

            if (request.Method == Method.Post && (response.StatusCode == HttpStatusCode.RedirectMethod ||
                                                  response.StatusCode == HttpStatusCode.Redirect)) {
                response.IsSuccessStatusCode = true;
                response.ResponseStatus = ResponseStatus.Completed;
                response.ErrorException = null;
            }

            TimeoutCheck(request, response);
            if (options.ThrowOnAnyError && response.ErrorException != null) {
                throw response.ErrorException;
            }

            return response;
        }

        private void SetForwardedHeader(IRestApiRequest request) {
            if (contextAccessor?.HttpContext?.Request == null) {
                return;
            }

            var ip = HttpContextUtility.GetRequestIpAddress(contextAccessor.HttpContext);
            if (string.IsNullOrWhiteSpace(ip)) {
                return;
            }

            request.AddHeader("X-Forwarded-For", ip);
            request.AddHeader("Forwarded", $"for={ip}");
        }

        public Task<RestResponse> ExecuteAsync(IRestApiRequest request) {
            return InnerExecuteAsync(request);
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(IRestApiRequest request) {
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);
            var result = DeserializeRestResponse<T>(request, response);

            return result;
        }

        public async Task<RestResponse> GetAsync(IRestApiRequest request) {
            request.Method = Method.Get;
            var response = await InnerExecuteAsync(request).ConfigureAwait(false);

            // TODO: move to InnerExecuteAsync and add as ErrorException?
            if (response.ResponseStatus == ResponseStatus.TimedOut && options.ThrowOnAnyError) {
                var timeouts = new int[] { request.Timeout, client.Options.MaxTimeout };
                var timeout = timeouts.Where(x => x > 0).Min();
                throw new TimeoutException($"Timeout of {timeout} exceeded");
            }

            if (response.ErrorException != null && options.ThrowOnAnyError) {
                LogError(request, response);
                // Request failed with status code Forbidden
                throw new RestApiException(response.ErrorMessage ?? response.ErrorException.Message, response.ErrorException);
            }

            if (!response.IsSuccessful && options.ThrowOnAnyError) {
                LogError(request, response);
                throw new RestApiException($"Request failed with status code {response.StatusCode}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response;
            } else {
                LogError(request, response);
                if (options.ThrowOnAnyError) {
                    throw new RestApiException("unsuccessful request--need better message");
                }

                return response;
            }
        }

        public async Task<RestResponse<T>> GetAsync<T>(IRestApiRequest request) where T : new() {
            var response = await GetAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var result = DeserializeRestResponse<T>(request, response);
                return result;
            }

            LogError(request, response);
            return RestResponse<T>.FromResponse(response);
        }

        public Task<T> GetWithCacheAsync<T>(IRestApiRequest request, TimeSpan? duration = null) where T : class, new() {
            var cacheKey = $"RestRequest::{client.BuildUri(request.RestRequest)}::{request.RestRequest.QueryParameters()}";
            return GetWithCacheAsync<T>(request, cacheKey, duration);
        }

        public async Task<T> GetWithCacheAsync<T>(IRestApiRequest request, string cacheKey, TimeSpan? duration = null) where T : class, new() {
            duration ??= TimeSpan.FromMinutes(10);
            var cacheOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration };

            var item = await Cache.GetValueAsync<T>(cacheKey, options.Serializer).ConfigureAwait(false);
            if (item == null) {
                var response = await GetAsync<T>(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    await Cache.SetValueAsync(cacheKey, response.Data, options.Serializer, cacheOptions).ConfigureAwait(false);
                    item = response.Data;
                } else {
                    LogError(request, response);
                    var ex = response.ErrorException;
                    if (ex != null && options.ThrowOnAnyError) {
                        throw ex;
                    }
                    return default;
                }
            }

            return item;
        }

        private void LogError(IRestApiRequest request, RestResponse response) {
            //Get the values of the parameters passed to the API
            string parameters = request.RestRequest.QueryParameters();

            //Set up the information message with the URL, 
            //the status code, and the parameters.
            string info = "Request to " + BuildUri(request) + " failed with response status " + response.ResponseStatus.ToString() + " status code "
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
