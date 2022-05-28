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
        readonly RestSharpClient client;

        public GitHubClient(ILogger<GitHubClient> logger) {
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            client = new RestSharpClient("https://api.github.com", logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public GitHubClient(ILogger<GitHubClient> logger, IDistributedCache cache) {
            client = new RestSharpClient("https://api.github.com", logger) {
                Serializer = new JsonNetSerializer(),
                Cache = cache
            };
        }

        public async Task<List<GitHubRepo>> GetReposAsync() {
            var request = new RestRequest("users/cortside/repos", Method.Get);
            var repos = await client.GetWithCacheAsync<List<GitHubRepo>>(request).ConfigureAwait(false);
            return repos;
        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
