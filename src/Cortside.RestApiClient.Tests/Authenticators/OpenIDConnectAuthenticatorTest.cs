using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using RestSharp;
using Xunit;

namespace Cortside.RestApiClient.Tests.Authenticators {
    public class OpenIDConnectAuthenticatorTest {
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
            var authenticator = new OpenIDConnectAuthenticator(tokenRequest)
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
            var authenticator = new OpenIDConnectAuthenticator("https://demo.duendesoftware.com", "client_credentials", "m2m", "secret", "api")
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
            var authenticator = new OpenIDConnectAuthenticator(tokenRequest)
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
    }
}
