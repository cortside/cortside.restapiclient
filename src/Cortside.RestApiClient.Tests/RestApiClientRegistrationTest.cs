using Cortside.AspNetCore.AccessControl;
using Cortside.RestApiClient.Tests.Clients.CatalogApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class RestApiClientRegistrationTest {

        [Fact]
        public void TestRestApiClientRegistration() {
            // arange
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            var services = new ServiceCollection();
            var idsConfig = configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();
            var clientConfig = configuration.GetSection("CatalogApi").Get<CatalogClientConfiguration>();
            clientConfig.Authentication = idsConfig.Authentication;
            services.AddRestApiClient<ICatalogClient, CatalogClient, CatalogClientConfiguration>(clientConfig);
            services.AddLogging(o => o.AddConsole());
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

            // act
            var sp = services.BuildServiceProvider();

            // assert
            var client = sp.GetRequiredService<ICatalogClient>();
            Assert.NotNull(client);
        }
    }
}
