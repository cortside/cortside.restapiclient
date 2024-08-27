using RestSharp.Authenticators;

namespace Cortside.RestApiClient.Authenticators {
    public interface IRestApiAuthenticator : IAuthenticator {
        /// <summary>
        /// Resets the cached authorization token to force reauthorization.  This is used by RestApiClient
        /// in the case of authorization failure (401) in cases like revoked tokens.
        /// Override this method to do something other than just set the internal token to null.
        /// </summary>
        void HandleUnauthorizedClientRequest();
    }
}
