using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cortside.MockServer;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Clients.LexisNexisApi;
using Cortside.RestApiClient.Tests.Mocks;
using Cortside.RestApiClient.Tests.Mocks.LexisNexis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.RestApiClient.Tests {
    public class XmlTest {
        private readonly LexisNexisClientConfiguration config;
        private readonly ITestOutputHelper output;

        public static MockHttpServer Server { get; set; }

        public XmlTest(ITestOutputHelper output) {
            this.output = output;

            var name = Guid.NewGuid().ToString();
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock<LexisNexisMock>()
                .AddMock<TestMock>()
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

        [Fact]
        public async Task ShouldHandleLargeXmlResponseWithoutDeserialization() {
            // arrange
            var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), config.ServiceUrl);
            var serializer = new XmlSerializer(typeof(Associates));

            // act
            var response = await client.GetAsync(new RestApiRequest("/large/xml", Method.Get));

            // assert
            Assert.True(response.IsSuccessful);
            Assert.True(!string.IsNullOrWhiteSpace(response.Content));

            using (var reader = new StringReader(response.Content!)) {
                var associates = (Associates)serializer.Deserialize(reader);

                Assert.NotNull(associates);
                Assert.NotNull(associates.Associate);
                Assert.Equal(88944, associates.Associate.Count);
            }
        }

        [Fact]
        public async Task ShouldHandleLargeXmlResponseWithDeserialization() {
            // arrange
            var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), config.ServiceUrl);
            var serializer = new XmlSerializer(typeof(Associates));

            // act
            var response = await client.GetAsync<Associates>(new RestApiRequest("/large/xml", Method.Get));

            // assert
            Assert.True(response.IsSuccessful);
            Assert.True(!string.IsNullOrWhiteSpace(response.Content));

            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Associate);
            Assert.Equal(88944, response.Data.Associate.Count);
        }
    }
}
