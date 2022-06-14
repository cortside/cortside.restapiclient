using Cortside.RestSharpClient.Authenticators.OpenIDConnect;

namespace Cortside.RestSharpClient.Tests.Clients.CatalogApi {
    public class CatalogClientConfiguration {
        public string ServiceUrl { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
