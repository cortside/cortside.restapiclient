using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly CatalogClientConfiguration config;

        public static MockHttpServer Server { get; set; }

        public OpenIDConnectAuthenticatorTest() {
            var name = Guid.NewGuid().ToString();

            // JWT tokens can be generated with defined values using this site: http://jwtbuilder.jamiekurtz.com/
            Server = new MockHttpServer(name)
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder<TestMock>()
                .ConfigureBuilder<DelegationGrantMock>();

            Server.WaitForStart();

            config = new CatalogClientConfiguration() {
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
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

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
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.True(authorization.Trim().Length > 1);
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
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.NotNull(authorization);
            Assert.True(authorization.Trim().Length > 1);
        }

        [Fact]
        public async Task ShouldFailToAuthenticateAsync() {
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

            // act
            await authenticator.Authenticate(client, request).ConfigureAwait(false);

            // assert
            var authorization = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.HttpHeader && x.Name == KnownHeaders.Authorization)?.Value?.ToString();
            Assert.Empty(authorization);
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
            var token = await authenticator.GetTokenAsync().ConfigureAwait(false);

            // assert
            Assert.Equal("Bearer foo-client_credentials-token", token);
        }
    }
}
