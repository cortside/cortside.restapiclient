using System.Threading.Tasks;
using Cortside.RestSharpClient.Tests.Clients;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class ClientTest {
        [Fact]
        public async Task ShouldGetRepositoriesDefaultCacheAsync() {
            // arrange
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            using (var client = new GitHubClient(new NullLogger<GitHubClient>(), cache)) {

                // act
                var repos = await client.GetReposAsync().ConfigureAwait(false);

                // assert
                Assert.NotEmpty(repos);
                Assert.Contains(repos, x => x.Name == "cortside.restsharpclient");
                Assert.NotNull(await client.Cache.GetAsync("RestRequest::https://api.github.com/users/cortside/repos::").ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task ShouldGetRepositoriesAsync() {
            // arrange
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var client = new GitHubClient(new NullLogger<GitHubClient>(), cache);

            // act
            var repos = await client.GetReposAsync().ConfigureAwait(false);

            // assert
            Assert.NotEmpty(repos);
            Assert.Contains(repos, x => x.Name == "cortside.restsharpclient");
            Assert.NotNull(await cache.GetAsync("RestRequest::https://api.github.com/users/cortside/repos::").ConfigureAwait(false));
        }
    }
}
