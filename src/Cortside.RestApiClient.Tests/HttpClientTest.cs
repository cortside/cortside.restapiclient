using System;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class HttpClientTest {
        private readonly HttpClient client;

        public HttpClientTest() {
            var name = Guid.NewGuid().ToString();
            var server = MockHttpServer.CreateBuilder(name)
                .AddMock<TestMock>()
                .Build();

            client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldUseHttpClientGetAsync() {
            var itemId = "wp-21";
            var url = $"/api/v1/items/{itemId}";

            var response = await this.client.GetAsync(url);

            Assert.NotNull(response);
            var content = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<CatalogItem>(content);
            Assert.Equal(itemId, item.Sku);
        }

        [Fact]
        public async Task ShouldUseHttpClientSendAsync() {
            var itemId = "wp-21";
            var url = $"/api/v1/items/{itemId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<CatalogItem>(content);

            Assert.Equal(itemId, item.Sku);
        }

        [Fact]
        public async Task ShouldUseRestSharpClient() {
            var itemId = "wp-21";
            var url = $"/api/v1/items/{itemId}";

            var rsc = new RestClient(client);
            var request = new RestRequest(url, Method.Get);
            var response = await rsc.ExecuteAsync<CatalogItem>(request);
            Assert.Contains(itemId, response.Data.Sku);
        }

        [Fact]
        public async Task ShouldUseRestApiClient() {
            var itemId = "wp-21";
            var url = $"/api/v1/items/{itemId}";

            var rac = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), new RestApiClientOptions(), client);
            var request = new RestApiRequest(url, Method.Get);
            var response = await rac.ExecuteAsync<CatalogItem>(request);
            Assert.Contains(itemId, response.Data.Sku);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldSetForwardHeaders(bool setForwardHeaders) {
            var itemId = "wp-21";
            var url = $"/api/v1/items/{itemId}";
            var options = new RestApiClientOptions() {
                EnableForwardHeaders = setForwardHeaders
            };
            var defaultContext = new DefaultHttpContext {
                Request = {
                    Path = url,
                    Host = new HostString("http://localhost"),
                    Headers = { { "X-FORWARDED-FOR", "192.168.0.1" } }
                }
            };
            var context = new HttpContextAccessor {
                HttpContext = defaultContext
            };

            var rac = new RestApiClient(new NullLogger<RestApiClient>(), context, options, client);
            var request = new RestApiRequest(url, Method.Get);
            var response = await rac.ExecuteAsync<CatalogItem>(request);
            var xForwardedFor = response.Request.Parameters.TryFind("X-Forwarded-For");
            var xForwarded = response.Request.Parameters.TryFind("Forwarded");
            if (setForwardHeaders) {
                Assert.NotNull(xForwardedFor);
                Assert.NotNull(xForwarded);
            } else {
                Assert.Null(xForwardedFor);
                Assert.Null(xForwarded);
            }
        }
    }
}
