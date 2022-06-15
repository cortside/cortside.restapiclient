namespace Cortside.RestApiClient.Authenticators.OpenIDConnect {
    public class TokenRequest {
        public string AuthorityUrl { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int SlidingExpiration { get; set; }
    }
}
