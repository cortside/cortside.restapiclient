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
        void RemoveParameter(Parameter parameter);
        RestApiRequest AddParameter(Parameter parameter);
        RestApiRequest AddParameter(string name, string value, bool encode = true);
        RestApiRequest AddParameter<T>(string name, T value, bool encode = true) where T : struct;
        RestApiRequest AddParameter(string name, object value, ParameterType type, bool encode = true);
        RestApiRequest AddStringBody(string body, DataFormat dataFormat);
        RestApiRequest AddStringBody(string body, string contentType);
        RestApiRequest AddJsonBody<T>(T obj, string contentType = "application/json") where T : class;
        RestApiRequest AddXmlBody<T>(T obj, string contentType = "application/xml", string xmlNamespace = "") where T : class;
    }
}
