using System.Collections.Generic;
using System.Net.Http;
using RestSharp;

namespace Cortside.RestApiClient {
    public interface IRestRequest {
        // comment Func<HttpResponseMessage, RestResponse> AdvancedResponseWriter { get; /*set;*/ }
        bool AlwaysMultipartFormData { get; set; }
        int Attempts { get; }
        HttpCompletionOption CompletionOption { get; set; }
        IReadOnlyCollection<FileParameter> Files { get; }
        string FormBoundary { get; set; }
        Method Method { get; set; }
        bool MultipartFormQuoteParameters { get; set; }

        //// comment Func<HttpResponseMessage, ValueTask> OnAfterRequest { get; set; }
        //// comment Action<RestResponse> OnBeforeDeserialization { get; set; }
        //// comment Func<HttpRequestMessage, ValueTask> OnBeforeRequest { get; set; }

        ParametersCollection Parameters { get; }
        DataFormat RequestFormat { get; set; }
        string Resource { get; set; }
        //comment Func<Stream, Stream> ResponseWriter { get; /*set;*/ }
        string RootElement { get; set; }
        int Timeout { get; set; }
        void RemoveParameter(Parameter parameter);
    }
}
