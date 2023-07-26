using System;
using System.Threading.Tasks;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
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
                Authenticator = new HttpBasicAuthenticator(clientConfiguration.Username, clientConfiguration.Password),
                XmlSerializer = true
            };
            client = new RestApiClient(logger, contextAccessor, options);
        }

        internal async Task<RestResponse<InstantIDResponseEx>> InstantIdAsync() {
            var request = new RestApiRequest("/WsIdentity/InstantID?ver_=1.87", Method.Post);
            var response = await client.ExecuteAsync<InstantIDResponseEx>(request).ConfigureAwait(false);
            return response;
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
