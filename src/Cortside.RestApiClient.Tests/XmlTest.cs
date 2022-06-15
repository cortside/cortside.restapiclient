using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Clients.LexisNexisApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class XmlTest {
        private readonly LexisNexisClientConfiguration config;

        public static MockHttpServer Server { get; set; }

        public XmlTest() {
            var name = Guid.NewGuid().ToString();
            Server = new MockHttpServer(name)
                .ConfigureBuilder<LexisNexisMock>();

            Server.WaitForStart();

            config = new LexisNexisClientConfiguration() {
                ServiceUrl = Server.Url,
                Username = "foo",
                Password = "bar"
            };
        }

        [Fact]
        public async Task ShouldGetItemAsync() {
            // arrange
            var client = new LexisNexisClient(new NullLogger<HttpStatusClient>(), config);

            // act
            var item = await client.InstantIdAsync().ConfigureAwait(false);

            // assert
            Assert.Equal("385133841", item.Data.Response.Result.UniqueId);
        }
    }
}
