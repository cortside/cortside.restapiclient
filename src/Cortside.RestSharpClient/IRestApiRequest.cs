using Polly;
using RestSharp;

namespace Cortside.RestSharpClient {
    public interface IRestApiRequest : IRestRequest {
        IAsyncPolicy<RestResponse> Policy { get; set; }
    }
}
