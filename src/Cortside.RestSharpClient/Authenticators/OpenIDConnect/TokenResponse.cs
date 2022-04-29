using System.Text.Json.Serialization;

namespace Cortside.RestSharpClient.Authenticators.OpenIDConnect {
    public class TokenResponse {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
