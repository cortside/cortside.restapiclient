using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests.Authenticators {
    public class DelegationTest {
        private readonly RestClient client;

        public static MockHttpServer Server { get; set; }

        public DelegationTest() {
            var name = Guid.NewGuid().ToString();

            // JWT tokens can be generated with defined values using this site: http://jwtbuilder.jamiekurtz.com/
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<TestMock>()
                .AddMock<DelegationGrantMock>()
                .Build();

            client = new RestClient(Server.Url);
        }

        public OpenIDConnectAuthenticator GetAuthenticator(string clientId, string token) {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = Server.Url,
                GrantType = "client_credentials",
                Scope = "scope",
                ClientId = clientId,
                ClientSecret = "secret"
            };

            // arrange
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues(token));
            var accessor = new HttpContextAccessor().SetHttpContext("host", "/path", header);

            var authenticator = new OpenIDConnectAuthenticator(accessor, tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                )
                .UseLogger(new NullLogger<OpenIDConnectAuthenticator>());

            return authenticator;
        }

        [Fact]
        public async Task ShouldNotDelegate_NoRequestToken() {
            // arrange
            var authenticator = GetAuthenticator("foo", "");
            var request = new RestRequest("/api/v1/items/1234", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.Contains("foo-client_credentials-token", authorization);
        }

        [Fact]
        public async Task ShouldDelegate() {
            // arrange
            var authenticator = GetAuthenticator("allow", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2OTA0ODIyNzEsImV4cCI6MTcyMjAxODI3MSwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsImdyYW50X3R5cGUiOiJkZWxlZ2F0aW9uIn0.x43mQWtHy_ubj36gMXuoYtqp6K32w9XVuooYXWWVQNo");
            var request = new RestRequest("/api/v1/items/1234", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.Contains("allow-delegation-token", authorization);
        }

        [Fact]
        public async Task ShouldNotDelegate() {
            // arrange
            var authenticator = GetAuthenticator("allow", "");
            var request = new RestRequest("/api/v1/items/1234", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.Contains("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2OTA0ODIyNzEsImV4cCI6MTcyMjAxODI3MSwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsImdyYW50X3R5cGUiOiJkZWxlZ2F0aW9uIn0.x43mQWtHy_ubj36gMXuoYtqp6K32w9XVuooYXWWVQNo", authorization);
        }
    }
}
