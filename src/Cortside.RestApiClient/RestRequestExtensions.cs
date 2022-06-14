using System.Linq;
using RestSharp;

namespace Cortside.RestApiClient {
    public static class RestRequestExtensions {
        public static string QueryParameters(this RestRequest request) {
            return string.Join(", ", request.Parameters.Select(x => x.Name + "=" + (x.Value ?? "NULL")).ToArray());
        }
    }
}
