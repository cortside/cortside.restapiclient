using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cortside.RestApiClient.Tests {
    public class HttpStatus0 {

        [Fact]
        public async Task BadSsl() {
            // arrange
            var client = new RestApiClient(new NullLogger<RestApiClient>(), new HttpContextAccessor(), "https://self-signed.badssl.com");
            var request = new RestApiRequest();

            // act
            var response = await client.GetAsync(request);

            // assert
            Assert.NotNull(response);
            Assert.Equal(0, (int)response.StatusCode);
        }
    }
}
