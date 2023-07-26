using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.GitHubApi {
    public class GitHubClient : IDisposable {
        private readonly RestApiClient client;
        private readonly RestApiClientOptions options;

        public GitHubClient(ILogger<GitHubClient> logger, IDistributedCache cache, IHttpContextAccessor contextAccessor) {
            options = new RestApiClientOptions("https://api.github.com") {
                Cache = cache
            };

            client = new RestApiClient(logger, contextAccessor, options);
        }

        public IDistributedCache Cache => options.Cache;

        public Task<List<GitHubRepo>> GetReposAsync() {
            var request = new RestApiRequest("users/cortside/repos", Method.Get);
            return client.GetWithCacheAsync<List<GitHubRepo>>(request, TimeSpan.FromMinutes(1));
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
