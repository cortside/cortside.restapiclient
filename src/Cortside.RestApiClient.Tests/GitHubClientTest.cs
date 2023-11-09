using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Tests.Clients.GitHubApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class GitHubClientTest {
        public static MockHttpServer Server { get; set; }

        public GitHubClientTest() {
            var name = Guid.NewGuid().ToString();
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<TestMock>()
                .Build();
        }

        [Fact]
        public async Task ShouldGetRepositoriesDefaultCacheAsync() {
            // arrange
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var client = new GitHubClient(new NullLogger<GitHubClient>(), cache, new HttpContextAccessor(), Server.Url);

            // act
            var repos = await client.GetReposAsync();

            // assert
            Assert.NotEmpty(repos);
            Assert.Contains(repos, x => x.Name == "cortside.restapiclient");
            Assert.NotNull(await client.Cache.GetAsync($"RestRequest::{Server.Url}/users/cortside/repos::"));
        }

        [Fact]
        public async Task ShouldGetRepositoriesAsync() {
            // arrange
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var client = new GitHubClient(new NullLogger<GitHubClient>(), cache, new HttpContextAccessor(), Server.Url);

            // act
            var repos = await client.GetReposAsync();

            // assert
            Assert.NotEmpty(repos);
            Assert.Contains(repos, x => x.Name == "cortside.restapiclient");
            Assert.NotNull(await cache.GetAsync($"RestRequest::{Server.Url}/users/cortside/repos::"));
        }
    }
}
