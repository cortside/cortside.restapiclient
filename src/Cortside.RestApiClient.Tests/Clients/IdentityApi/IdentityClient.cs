using System;
using System.Threading.Tasks;
using Cortside.RestApiClient;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using HttpTracer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.IdentityApi {
    public class IdentityClient : IDisposable {
        private readonly RestApiClient apiClient;
        private readonly ILogger<IdentityClient> logger;
        private readonly RecaptchaConfiguration configuration;

        public IdentityClient(ILogger<IdentityClient> logger, RecaptchaConfiguration config, IHttpContextAccessor contextAccessor) {
            this.logger = logger;
            configuration = config;

            var options = new RestApiClientOptions {
                BaseUrl = new Uri(config.ServiceUrl),
                FollowRedirects = true,
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
                ThrowOnAnyError = false,
                Options = {
                    ConfigureMessageHandler = handler => new HttpTracerHandler(handler, new OutputLogger(logger), HttpMessageParts.All)
                }
            };
            apiClient = new RestApiClient(logger, contextAccessor, options);
        }

        public Task<TokenResponse> GetValidationTokenAsync(string recaptchaRequestToken, Guid applicationGuid, string client, string version) {
            return GetTokenAsync(recaptchaRequestToken, applicationGuid, client, version);
        }

        private async Task<TokenResponse> GetTokenAsync(string recaptchaRequestToken, Guid applicationGuid, string client, string version) {
            try {
                var tokenRequest = configuration.Authentication;
                var request = new RestApiRequest("/connect/token", Method.Post)
                    .AddParameter("site_secret", configuration.Secret)
                    .AddParameter("recaptcha_token", recaptchaRequestToken)
                    .AddParameter("remote_ip", "123.123.123.123")
                    .AddParameter("resource_id", applicationGuid)
                    .AddParameter("client", client)
                    .AddParameter("version", version)
                    .AddParameter("grant_type", tokenRequest.GrantType)
                    .AddParameter("scope", tokenRequest.Scope)
                    .AddParameter("client_id", tokenRequest.ClientId)
                    .AddParameter("client_secret", tokenRequest.ClientSecret);

                var response = await apiClient.ExecuteAsync<TokenResponse>(request).ConfigureAwait(false);

                if (!response.IsSuccessful) {
                    logger.LogError(response.ErrorException, $"Access Token cannot obtain, process terminate");
                    logger.LogError($"IdentityServer response content on failure to obtain recaptcha token: {response.Content}");
                    throw new InvalidOperationException("unable to obtain recaptcha token from identity server");
                }

                return response.Data;
            } catch (Exception ex) {
                logger.LogError(ex, $"Failed to retrieve token with exception {ex.Message}");
                throw;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            apiClient?.Dispose();
        }
    }
}
