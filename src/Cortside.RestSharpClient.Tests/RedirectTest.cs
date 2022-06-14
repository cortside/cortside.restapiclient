using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestSharpClient.Tests.Clients.CatalogApi;
using Cortside.RestSharpClient.Tests.Clients.HttpStatusApi;
using Cortside.RestSharpClient.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class RedirectTest {
        private readonly CatalogClientConfiguration config;

        public static MockHttpServer Server { get; set; }

        public RedirectTest() {
            var name = Guid.NewGuid().ToString();
            Server = new MockHttpServer(name)
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder<TestMock>();

            Server.WaitForStart();

            config = new CatalogClientConfiguration() {
                ServiceUrl = Server.Url,
                Authentication = new Cortside.RestSharpClient.Authenticators.OpenIDConnect.TokenRequest() {
                    AuthorityUrl = Server.Url,
                    ClientId = "foo",
                    ClientSecret = "bar",
                    GrantType = "client_credentials",
                    Scope = "catalog-api",
                    SlidingExpiration = 30
                }
            };
        }

        [Fact]
        public async Task ShouldGetItemAsync() {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.GetItemAsync("1234").ConfigureAwait(false);

            // assert
            Assert.Equal("1234", item.Data.Sku);
        }

        [Fact]
        public async Task ShouldFollowRedirectAsync() {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.CreateItemAsync().ConfigureAwait(false);

            // assert
            Assert.Equal("1234", item.Data.Sku);
        }
    }
}
