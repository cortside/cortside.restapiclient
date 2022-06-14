using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestSharpClient.Tests.Clients;
using Cortside.RestSharpClient.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp;
using Xunit;

namespace Cortside.RestSharpClient.Tests {
    public class TimeoutTest {
        public static MockHttpServer Server { get; set; }

        public TimeoutTest() {
            var name = Guid.NewGuid().ToString();
            Server = new MockHttpServer(name)
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder<TestMock>();

            Server.WaitForStart();
        }

        [Fact]
        public Task ShouldThrowTimeoutExceptionAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = true
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            return Assert.ThrowsAsync<TimeoutException>(() => client.GetTimeoutAsync());
        }

        [Fact]
        public Task ShouldThrowExceptionForGetClientMethodAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = false
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            return Assert.ThrowsAsync<TimeoutException>(() => client.GetTimeoutAsync());
        }

        [Fact]
        public async Task ShouldThrowTimeoutExceptionOnThrowOnAnyErrorAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = true
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            RestResponse response = null;
            await Assert.ThrowsAsync<TimeoutException>(async () => response = await client.ExecuteTimeoutAsync());
            Assert.Null(response);
        }

        [Fact]
        public async Task ShouldNotThrowTimeoutExceptionOnThrowOnAnyErrorAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = false
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), options);

            // act
            RestResponse response = await client.ExecuteTimeoutAsync();
            Assert.False(response.IsSuccessful);
        }
    }
}
