using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestSharp {
    public interface IRestRequest {
        Func<HttpResponseMessage, RestResponse> AdvancedResponseWriter { get; set; }
        bool AlwaysMultipartFormData { get; set; }
        int Attempts { get; }
        HttpCompletionOption CompletionOption { get; set; }
        IReadOnlyCollection<FileParameter> Files { get; }
        string FormBoundary { get; set; }
        Method Method { get; set; }
        bool MultipartFormQuoteParameters { get; set; }
        Func<HttpResponseMessage, ValueTask> OnAfterRequest { get; set; }
        Action<RestResponse> OnBeforeDeserialization { get; set; }
        Func<HttpRequestMessage, ValueTask> OnBeforeRequest { get; set; }
        ParametersCollection Parameters { get; }
        DataFormat RequestFormat { get; set; }
        string Resource { get; set; }
        Func<Stream, Stream> ResponseWriter { get; set; }
        string RootElement { get; set; }
        int Timeout { get; set; }

        RestRequest AddParameter(Parameter parameter);
        void RemoveParameter(Parameter parameter);
    }
}