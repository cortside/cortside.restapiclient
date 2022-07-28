using System;
using System.Net;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
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
        public async Task ShouldGetItemAsync() {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.GetItemAsync("1234").ConfigureAwait(false);

            // assert
            Assert.Equal("1234", item.Data.Sku);
        }

        [Theory]
        [InlineData(false, 201)]
        [InlineData(true, 200)]
        public async Task ShouldFollowRedirectAsync(bool followRedirects, int statusCode) {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.CreateItemAsync(followRedirects).ConfigureAwait(false);

            // assert
            Assert.True(item.IsSuccessful);
            Assert.Equal(statusCode, (int)item.StatusCode);
            Assert.Equal("1234", item.Data.Sku);
        }

        [Theory]
        [InlineData(false, 303)]
        [InlineData(true, 200)]
        public async Task ShouldHandle303AsSuccessful(bool followRedirects, int statusCode) {
            // arrange
            var client = new CatalogClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.SearchItemsAsync(followRedirects).ConfigureAwait(false);

            // assert
            Assert.Equal(statusCode, (int)item.StatusCode);
            Assert.True(item.IsSuccessful);
            Assert.Equal(followRedirects, item.Content.Length > 0);
        }

        [Theory]
        [InlineData(false, 303)]
        [InlineData(true, 200)]
        public async Task RestSharpRedirect(bool followRedirects, int statusCode) {
            // arrange
            var options = new RestSharp.RestClientOptions(Server.Url) {
                FollowRedirects = followRedirects,
            };
            var client = new RestSharp.RestClient(options);

            Assert.False(options.ThrowOnAnyError);

            // act
            var request = new RestSharp.RestRequest("/api/v1/items/search", RestSharp.Method.Post);
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            // this is what RestApiClient does now to alter the hanlding
            if (request.Method == Method.Post && (response.StatusCode == HttpStatusCode.RedirectMethod || response.StatusCode == HttpStatusCode.Redirect)) {
                response.IsSuccessful = true;
                response.ErrorException = null;
            }

            // assert
            Assert.Equal(statusCode, (int)response.StatusCode);
            Assert.True(response.IsSuccessful);
            Assert.Equal(followRedirects, response.Content.Length > 0);
        }
    }
}
