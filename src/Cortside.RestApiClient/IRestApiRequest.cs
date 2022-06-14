using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Polly;
using RestSharp;

namespace Cortside.RestApiClient {
    public interface IRestApiRequest {
        bool AlwaysMultipartFormData { get; set; }
        int Attempts { get; }
        HttpCompletionOption CompletionOption { get; set; }
        IReadOnlyCollection<FileParameter> Files { get; }
        bool? FollowRedirects { get; set; }
        string FormBoundary { get; set; }
        Method Method { get; set; }
        bool MultipartFormQuoteParameters { get; set; }
        ParametersCollection Parameters { get; }
        IAsyncPolicy<RestResponse> Policy { get; set; }
        DataFormat RequestFormat { get; set; }
        string Resource { get; set; }
        RestRequest RestRequest { get; }
        string RootElement { get; set; }
        int Timeout { get; set; }
        RestApiRequest AddFile(string name, byte[] bytes, string filename, string contentType = null);
        RestApiRequest AddFile(string name, Func<Stream> getFile, string fileName, string contentType = null);
        RestApiRequest AddFile(string name, string path, string contentType = null);
        RestApiRequest AddHeader(string name, string value);
        RestApiRequest AddParameter(Parameter parameter);
        void RemoveParameter(Parameter parameter);
    }
}
