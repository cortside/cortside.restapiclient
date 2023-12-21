using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Clients.LexisNexisApi;
using Cortside.RestApiClient.Tests.Mocks.LexisNexis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class XmlTest {
        private readonly LexisNexisClientConfiguration config;

        public static MockHttpServer Server { get; set; }

        public XmlTest() {
            var name = Guid.NewGuid().ToString();
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock<LexisNexisMock>()
                .Build();

            config = new LexisNexisClientConfiguration() {
                ServiceUrl = Server.Url,
                Username = "foo",
                Password = "bar"
            };
        }

        [Fact]
        public async Task ShouldGetItemAsync() {
            // arrange
            var client = new LexisNexisClient(new NullLogger<HttpStatusClient>(), config, new HttpContextAccessor());

            // act
            var item = await client.InstantIdAsync();

            // assert
            Assert.Equal("385133841", item.Data.Response.Result.UniqueId);
        }

        [Fact]
        public async Task ShouldDeserializeAllXmlChildren() {
            // arrange
            var client = new LexisNexisClient(new NullLogger<HttpStatusClient>(), config, new HttpContextAccessor());

            // act
            var response = await client.VerificationOfOccupancyAsync();

            // assert
            Assert.Equal(17, response.Response.Result.AttributeGroup.Attributes.Attribute.Count);
        }
    }
}
