using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace EnerBank.Application.IdentityServerClient {
    public class RecaptchaConfiguration {
        public string ServiceUrl { get; set; }
        public string Secret { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
