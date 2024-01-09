using System;
using System.Net;
using System.Threading.Tasks;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Mocks.LexisNexis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;

namespace Cortside.RestApiClient.Tests.Clients.LexisNexisApi {
    public class LexisNexisClient : IDisposable {
        readonly RestApiClient client;

        public LexisNexisClient(ILogger<HttpStatusClient> logger, LexisNexisClientConfiguration clientConfiguration, IHttpContextAccessor contextAccessor) {
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(clientConfiguration.ServiceUrl),
                Authenticator = new HttpBasicAuthenticator(clientConfiguration.Username, clientConfiguration.Password)
            };
            client = new RestApiClient(logger, contextAccessor, options);
        }

        public async Task<RestResponse<InstantIDResponseEx>> InstantIdAsync() {
            var request = new RestApiRequest("/WsIdentity/InstantID?ver_=1.87", Method.Post);
            var response = await client.ExecuteAsync<InstantIDResponseEx>(request).ConfigureAwait(false);
            return response;
        }

        public async Task<VerificationOfOccupancyResponse> VerificationOfOccupancyAsync() {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest("/WsIdentity/VerificationOfOccupancy", Method.Post);
            request.Timeout = 30000;

            var response = await client.ExecuteAsync<VerificationOfOccupancyResponse>(RestApiRequest.From(request)).ConfigureAwait(false);
            return response.Data;
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
