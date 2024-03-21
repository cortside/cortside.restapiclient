#pragma warning disable VSTHRD200

using System.Threading.Tasks;
using RestSharp;

namespace Cortside.RestApiClient.Authenticators {
    public abstract class RestApiAuthenticator : IRestApiAuthenticator {
        protected RestApiAuthenticator(string token) => Token = token;

        protected string Token { get; set; }

        protected abstract ValueTask<Parameter> GetAuthenticationParameter(string accessToken);

        public async ValueTask Authenticate(IRestClient client, RestRequest request)
            => request.AddOrUpdateParameter(await GetAuthenticationParameter(Token).ConfigureAwait(false));

        public virtual void HandleUnauthorizedClientRequest() {
            Token = null;
        }
    }
}
