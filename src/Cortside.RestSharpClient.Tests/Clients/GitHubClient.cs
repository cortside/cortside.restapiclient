using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cortside.RestSharpClient.Tests.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.RestSharpClient.Tests.Clients {
    public class GitHubClient : IDisposable {
        private readonly RestApiClient client;
        private readonly RestApiClientOptions options;

        public GitHubClient(ILogger<GitHubClient> logger, IDistributedCache cache) {
            options = new RestApiClientOptions("https://api.github.com") {
                Cache = cache
            };

            client = new RestApiClient(options, logger);
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
