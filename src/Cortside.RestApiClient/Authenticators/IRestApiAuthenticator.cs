using RestSharp.Authenticators;

namespace Cortside.RestApiClient.Authenticators {
    public interface IRestApiAuthenticator : IAuthenticator {
        void ClearToken();
    }
}
