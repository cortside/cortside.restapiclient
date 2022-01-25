namespace Cortside.RestSharpClient.Authenticators.OpenIDConnect {
    public class TokenRequest {
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Url { get; set; }
        public int SlidingExpiration { get; set; }
    }
}
