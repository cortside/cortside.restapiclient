using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.RestSharpClient.Tests.Clients;
using Cortside.RestSharpClient.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class RedirectTest {
        public static MockHttpServer Server { get; set; }

        public RedirectTest() {
            var name = Guid.NewGuid().ToString();
            Server = new MockHttpServer(name)
                .ConfigureBuilder<TestMock>();

            Server.WaitForStart();
        }

        [Fact]
        public async Task ShouldGetItemAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = false
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            var item = await client.GetItemAsync("1234").ConfigureAwait(false);

            // assert
            Assert.Equal("1234", item.Data.Sku);
        }

        [Fact]
        public async Task ShouldFollowRedirectAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = false
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            var item = await client.CreateItemAsync().ConfigureAwait(false);

            // assert
            Assert.Equal("1234", item.Data.Sku);
        }
    }
}
