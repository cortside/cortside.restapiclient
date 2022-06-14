#pragma warning disable RCS1085 // Use auto-implemented property.
#pragma warning disable IDE0032 // Use auto-implemented property.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Polly;
using RestSharp;

namespace Cortside.RestApiClient {
    public class RestApiRequest : IRestApiRequest {
        private readonly RestRequest request;

        public RestApiRequest() {
            request = new RestRequest();
        }

        private RestApiRequest(RestRequest request) {
            this.request = request;
        }

        public RestApiRequest(string resource, Method method) {
            request = new RestRequest(resource, method);
        }

        public static RestApiRequest From(RestRequest request) {
            return new RestApiRequest(request);
        }

        public IAsyncPolicy<RestResponse> Policy { get; set; }

        public bool AlwaysMultipartFormData {
            get => request.AlwaysMultipartFormData;
            set => request.AlwaysMultipartFormData = value;
        }

        public int Attempts => request.Attempts;

        public HttpCompletionOption CompletionOption {
            get => request.CompletionOption;
            set => request.CompletionOption = value;
        }

        public IReadOnlyCollection<FileParameter> Files => request.Files;

        public string FormBoundary {
            get => request.FormBoundary;
            set => request.FormBoundary = value;
        }

        public Method Method {
            get => request.Method;
            set => request.Method = value;
        }

        public bool MultipartFormQuoteParameters {
            get => request.MultipartFormQuoteParameters;
            set => request.MultipartFormQuoteParameters = value;
        }

        public ParametersCollection Parameters => request.Parameters;

        public DataFormat RequestFormat {
            get => request.RequestFormat;
            set => request.RequestFormat = value;
        }
        public string Resource {
            get => request.Resource;
            set => request.Resource = value;
        }

        public string RootElement {
            get => request.RootElement;
            set => request.RootElement = value;
        }
        public int Timeout {
            get => request.Timeout;
            set => request.Timeout = value;
        }

        public RestRequest RestRequest => request;

        public RestApiRequest AddParameter(Parameter parameter) {
            request.AddParameter(parameter);
            return this;
        }

        public void RemoveParameter(Parameter parameter) {
            request.RemoveParameter(parameter);
        }

        public RestApiRequest AddHeader(string name, string value) {
            return AddParameter(new HeaderParameter(name, value));
        }

        public RestApiRequest AddFile(string name, string path, string contentType = null) {
            RestRequest.AddFile(name, path, contentType);
            return this;
        }

        public RestApiRequest AddFile(string name, byte[] bytes, string filename, string contentType = null) {
            RestRequest.AddFile(name, bytes, filename, contentType);
            return this;
        }

        public RestApiRequest AddFile(string name, Func<Stream> getFile, string fileName, string contentType = null) {
            RestRequest.AddFile(name, getFile, fileName, contentType);
            return this;
        }

        public bool? FollowRedirects { get; set; }
    }
}
