using System;
using System.Threading.Tasks;
using Cortside.Common.Testing.Logging.Xunit;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Cortside.RestApiClient.Tests.Mocks;
using EnerBank.Application.IdentityServerClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.RestApiClient.Tests.Authenticators {
    public class RecaptchaTest {
        private readonly IdentityClient client;

        public static MockHttpServer Server { get; set; }

        public RecaptchaTest(ITestOutputHelper output) {
            var name = Guid.NewGuid().ToString();

            // Create a logger factory with a debug provider
            var loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Cortside.Common", LogLevel.Trace)
                    .AddXunit(output);
            });

            // Create a logger with the category name of the current class
            var logger = loggerFactory.CreateLogger<IdentityClient>();

            // JWT tokens can be generated with defined values using this site: http://jwtbuilder.jamiekurtz.com/
            Server = MockHttpServer.CreateBuilder(name)
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<TestMock>()
                .AddMock<RecaptchaGrantMock>()
                .Build();

            var config = new RecaptchaConfiguration() {
                ServiceUrl = Server.Url,
                Secret = "recaptcha_site_secret",
                Authentication = new TokenRequest() {
                    ClientId = "foo-web",
                    ClientSecret = "client_secret",
                    GrantType = "recaptcha",
                    Scope = "api",
                    AuthorityUrl = Server.Url
                }
            };

            client = new IdentityClient(logger, config, new HttpContextAccessor());
        }

        [Fact]
        public async Task ShouldGetToken() {
            // arrange

            // act
            var response = await client.GetValidationTokenAsync("recaptcha_request_token", Guid.NewGuid(), "application-web", "1.0");

            // assert
            Assert.Contains("foo-recaptcha-token", response.AccessToken);
        }
    }
}
