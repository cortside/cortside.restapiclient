using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
namespace Cortside.RestApiClient.Tests {
    public class SerializationTest {
        private readonly CatalogClientConfiguration config;
        public static MockHttpServer Server { get; set; }

        public SerializationTest() {
            var name = Guid.NewGuid().ToString();
            Server = new MockHttpServer(name)
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder<TestMock>();

            Server.WaitForStart();

            config = new CatalogClientConfiguration() {
                ServiceUrl = Server.Url,
                Authentication = new Cortside.RestApiClient.Authenticators.OpenIDConnect.TokenRequest() {
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
        public Task ShouldThrowExceptionOnSerializationErrorAsync() {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config, new HttpContextAccessor(), true);

            // act
            return Assert.ThrowsAnyAsync<Exception>(async () => await client.ModelMismatchAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ShouldFailOnSerializationErrorAsync() {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config, new HttpContextAccessor(), false);

            // act
            var response = await client.ModelMismatchAsync().ConfigureAwait(false);
            Assert.NotNull(response.ErrorException);
        }
    }
}
