using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Cortside.RestApiClient.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests.Authenticators {
    public class OpenIDConnectAuthenticatorTest {
        public static MockHttpServer Server { get; set; }

        public OpenIDConnectAuthenticatorTest() {
            var name = Guid.NewGuid().ToString();

            // JWT tokens can be generated with defined values using this site: http://jwtbuilder.jamiekurtz.com/
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<TestMock>()
                .AddMock<DelegationGrantMock>()
                .Build();

            var config = new CatalogClientConfiguration() {
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

        public static IHttpContextAccessor GetHttpContext(string host, string requestPath, KeyValuePair<string, StringValues> header) {
            var context = new DefaultHttpContext {
                Request = {
                    Path = requestPath,
                    Host = new HostString(host),
                    Headers = {  }
                }
            };

            context.Request.Headers.Add(header.Key, header.Value);

            var obj = new HttpContextAccessor();
            obj.HttpContext = context;
            return obj;
        }

        [Fact]
        public async Task ShouldDelegate() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = Server.Url,
                GrantType = "client_credentials",
                Scope = "foo",
                ClientId = "foo",
                ClientSecret = "secret"
            };

            // arrange
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("some-token"));
            var context = GetHttpContext("host", "/path", header);

            var authenticator = new OpenIDConnectAuthenticator(context, tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                )
                .UseLogger(new NullLogger<OpenIDConnectAuthenticator>());

            var client = new RestClient(Server.Url);
            var request = new RestRequest("/api/v1/items/1234", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.Contains("foo-client_credentials-token", authorization);
        }

        [Fact]
        public async Task ShouldAuthenticateWithTokenRequestAsync() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = "https://demo.duendesoftware.com",
                GrantType = "client_credentials",
                Scope = "api",
                ClientId = "m2m",
                ClientSecret = "secret"
            };

            // arrange
            var authenticator = new OpenIDConnectAuthenticator(null, tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                )
                .UseLogger(new NullLogger<OpenIDConnectAuthenticator>());

            var client = new RestClient("http://api.github.com");
            var request = new RestRequest("foo", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.True(authorization.Trim().Length > 1);
        }

        [Fact]
        public async Task ShouldAuthenticateWithTokenRequestWithCachedTokenAsync() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = "https://demo.duendesoftware.com",
                GrantType = "client_credentials",
                Scope = "api",
                ClientId = "m2m",
                ClientSecret = "secret"
            };

            // arrange
            var authenticator = new OpenIDConnectAuthenticator(null, tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                )
                .UseLogger(new NullLogger<OpenIDConnectAuthenticator>());

            var client = new RestClient("http://api.github.com");

            for (var i = 0; i < 10; i++) {
                var request = new RestRequest("foo", Method.Get);

                // act
                await authenticator.Authenticate(client, request);

                // assert
                var authorization = request.Parameters
                    .FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)
                    ?.Value?.ToString();
                Assert.NotNull(authorization);
                Assert.True(authorization.Trim().Length > 1);
                Assert.StartsWith("Bearer ", authorization);
                var token = authorization.Right(authorization.Length - 7);
                Assert.Equal(3, token.Split(".").Length);
            }
        }

        [Fact]
        public async Task ShouldAddAuthorizationHeaderAsync() {
            // arrange
            var authenticator = new OpenIDConnectAuthenticator(new HttpContextAccessor(), "https://demo.duendesoftware.com", "client_credentials", "m2m", "secret", "api")
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                );

            var client = new RestClient("http://api.github.com");
            var request = new RestRequest("foo", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.True(authorization.Trim().Length > 1);
        }

        [Fact]
        public Task ShouldFailToAuthenticateAsync() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = "https://demo.duendesoftware.com",
                GrantType = "client_credentials",
                Scope = "api",
                ClientId = "m2m",
                ClientSecret = "xxx"
            };

            // arrange
            var authenticator = new OpenIDConnectAuthenticator(new HttpContextAccessor(), tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                );

            var client = new RestClient("http://api.github.com");
            var request = new RestRequest("foo", Method.Get);

            // act && assert
            return Assert.ThrowsAsync<AuthenticationException>(async () => await authenticator.Authenticate(client, request));
        }

        [Fact]
        public Task ShouldFailToAuthenticateWithCallWithThrowAsync() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = "https://demo.duendesoftware.com",
                GrantType = "client_credentials",
                Scope = "api",
                ClientId = "m2m",
                ClientSecret = "xxx"
            };

            // arrange
            var authenticator = new OpenIDConnectAuthenticator(new HttpContextAccessor(), tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                );

            var options = new RestApiClientOptions() {
                BaseUrl = new Uri("http://api.github.com"),
                Authenticator = authenticator,
                ThrowOnAnyError = true
            };
            var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), options);
            var request = new RestApiRequest("foo", Method.Get);

            // act && assert
            return Assert.ThrowsAsync<AuthenticationException>(async () => await client.GetAsync(request));
        }

        [Fact]
        public async Task ShouldFailToAuthenticateWithCallWithNoThrowAsync() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = "https://demo.duendesoftware.com",
                GrantType = "client_credentials",
                Scope = "api",
                ClientId = "m2m",
                ClientSecret = "xxx"
            };

            // arrange
            var authenticator = new OpenIDConnectAuthenticator(new HttpContextAccessor(), tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                );

            var options = new RestApiClientOptions() {
                BaseUrl = new Uri("http://api.github.com"),
                Authenticator = authenticator,
                ThrowOnAnyError = false
            };
            var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), options);
            var request = new RestApiRequest("foo", Method.Get);

            // act
            var response = await client.GetAsync(request);

            // assert
            Assert.False(response.IsSuccessful);
        }

        [Fact]
        public async Task ShouldHandleNonJwtToken() {
            var tokenRequest = new TokenRequest {
                AuthorityUrl = Server.Url,
                GrantType = "client_credentials",
                Scope = "delegation",
                ClientId = "foo",
                ClientSecret = "secret"
            };

            // arrange
            var header = new KeyValuePair<string, StringValues>("Authorization", new StringValues("some-token"));
            var context = GetHttpContext("host", "/path", header);

            var authenticator = new OpenIDConnectAuthenticator(context, tokenRequest)
                .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                    .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                )
                .UseLogger(new NullLogger<OpenIDConnectAuthenticator>());

            var client = new RestClient(Server.Url);
            var request = new RestRequest("/api/v1/items/1234", Method.Get);

            // act
            await authenticator.Authenticate(client, request);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.Equal("Bearer foo-client_credentials-token", authorization.ToString());
        }
    }
}
