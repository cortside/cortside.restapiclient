using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using RestSharp;

namespace Cortside.RestSharpClient {
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


        public Func<HttpResponseMessage, RestResponse> AdvancedResponseWriter {
            get => request.AdvancedResponseWriter;
            set => throw new NotImplementedException();
        }

        public bool AlwaysMultipartFormData {
            get => request.AlwaysMultipartFormData;
            set => request.AlwaysMultipartFormData = value;
        }

        public int Attempts => request.Attempts;

        public HttpCompletionOption CompletionOption {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IReadOnlyCollection<FileParameter> Files => request.Files;

        public string FormBoundary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Method Method {
            get => request.Method;
            set => request.Method = value;
        }

        public bool MultipartFormQuoteParameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<HttpResponseMessage, ValueTask> OnAfterRequest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<RestResponse> OnBeforeDeserialization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<HttpRequestMessage, ValueTask> OnBeforeRequest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ParametersCollection Parameters => request.Parameters;

        public DataFormat RequestFormat {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public string Resource {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public Func<Stream, Stream> ResponseWriter {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public string RootElement {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
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
    }
}
