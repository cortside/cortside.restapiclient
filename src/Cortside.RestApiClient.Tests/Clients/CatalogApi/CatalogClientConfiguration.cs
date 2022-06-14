using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace Cortside.RestApiClient.Tests.Clients.CatalogApi {
    public class CatalogClientConfiguration {
        public string ServiceUrl { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
