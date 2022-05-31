using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Cortside.RestSharpClient.Tests.Clients {
    public class GitHubClient : IDisposable {
        private readonly RestSharpClient client;
        private readonly IDistributedCache cache;

        public GitHubClient(ILogger<GitHubClient> logger) {
            cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            client = new RestSharpClient("https://api.github.com", logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public GitHubClient(ILogger<GitHubClient> logger, IDistributedCache cache) {
            this.cache = cache;
            client = new RestSharpClient("https://api.github.com", logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public IDistributedCache Cache => cache;

        public Task<List<GitHubRepo>> GetReposAsync() {
            var request = new RestRequest("users/cortside/repos", Method.Get);
            return client.GetWithCacheAsync<List<GitHubRepo>>(request, TimeSpan.FromMinutes(1));
        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
