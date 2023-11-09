using System;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class TimeoutTest {
        public static MockHttpServer Server { get; set; }

        public TimeoutTest() {
            var name = Guid.NewGuid().ToString();
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<TestMock>()
                .Build();
        }

        [Fact]
        public Task ShouldThrowTimeoutExceptionAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = true
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

            // act
            return Assert.ThrowsAsync<TimeoutException>(() => client.GetTimeoutAsync());
        }

        [Fact]
        public async Task ShouldNotThrowExceptionForGetClientMethodAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = false
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

            // act
            var response = await client.GetTimeoutAsync();

            Assert.Null(response);
        }

        [Fact]
        public Task ShouldThrowExceptionForGetClientMethodAsync() {
            // arrange
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(Server.Url),
                ThrowOnAnyError = true
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

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
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

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
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

            // act
            RestResponse response = await client.ExecuteTimeoutAsync();
            Assert.False(response.IsSuccessful);
        }
    }
}
