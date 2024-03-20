using System;
using System.Threading.Tasks;
using Cortside.RestApiClient.Tests.Clients;
using Cortside.RestApiClient.Tests.Clients.HttpStatusApi;
using Cortside.RestApiClient.Tests.ResponseProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class RetryTest {
        [Fact]
        public async Task ShouldRetryAsync() {
            // arrange
            var server = WireMockServer.Start();
            server.Given(Request.Create().WithPath("/200retry").UsingGet()).RespondWith(new AlternateFailureResponse());

            string url = "http://localhost:" + server.Ports[0];

            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), url, new HttpContextAccessor());

            // act
            var repos = await client.Get200RetryAsync();

            // assert
            Assert.NotEmpty(repos);
        }

        [Fact]
        public async Task ShouldReauthenticateAsync() {
            // arrange
            var server = WireMockServer.Start();
            server.Given(Request.Create().WithPath("/401").UsingGet()).RespondWith(Response.Create().WithStatusCode(401));
            string url = "http://localhost:" + server.Ports[0];

            var authenticator = new TestAuthenticator();
            var options = new RestApiClientOptions() {
                BaseUrl = new Uri(url),
                Authenticator = authenticator
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

            // act
            var response = await client.Get401Async();

            // assert
            Assert.False(response.IsSuccessful);
            Assert.Null(authenticator.CachedToken);
        }

        [Fact]
        public async Task ShouldNotReauthenticateAsync() {
            // arrange
            var server = WireMockServer.Start();
            server.Given(Request.Create().WithPath("/200").UsingGet()).RespondWith(Response.Create().WithStatusCode(200));
            string url = "http://localhost:" + server.Ports[0];

            var authenticator = new TestAuthenticator();
            var options = new RestApiClientOptions() {
                BaseUrl = new Uri(url),
                Authenticator = authenticator
            };
            var client = new HttpStatusClient(new NullLogger<HttpStatusClient>(), new HttpContextAccessor(), options);

            // act
            var response = await client.Get200Async();

            // assert
            Assert.True(response.IsSuccessful);
            Assert.NotNull(authenticator.CachedToken);
        }

    }
}
