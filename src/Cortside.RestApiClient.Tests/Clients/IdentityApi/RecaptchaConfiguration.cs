using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace Cortside.RestApiClient.Tests.Clients.IdentityApi {
    public class RecaptchaConfiguration {
        public string ServiceUrl { get; set; }
        public string Secret { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
