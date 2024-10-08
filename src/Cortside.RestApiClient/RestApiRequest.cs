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
        public TimeSpan? Timeout {
            get => request.Timeout;
            set => request.Timeout = value;
        }

        public RestRequest RestRequest => request;

        public void RemoveParameter(Parameter parameter) {
            request.RemoveParameter(parameter);
        }

        public RestApiRequest AddHeader(string name, string value) {
            return AddParameter(new HeaderParameter(name, value));
        }

        public RestApiRequest AddFile(string name, string path, ContentType contentType = null) {
            RestRequest.AddFile(name, path, contentType);
            return this;
        }

        public RestApiRequest AddFile(string name, byte[] bytes, string filename, ContentType contentType = null) {
            RestRequest.AddFile(name, bytes, filename, contentType);
            return this;
        }

        public RestApiRequest AddFile(string name, Func<Stream> getFile, string fileName, ContentType contentType = null) {
            RestRequest.AddFile(name, getFile, fileName, contentType);
            return this;
        }

        public bool? FollowRedirects { get; set; }

        public RestApiRequest AddParameter(Parameter parameter) {
            request.AddParameter(parameter);
            return this;
        }

        public RestApiRequest AddParameter(string name, string value, bool encode = true) {
            request.AddParameter(new GetOrPostParameter(name, value, encode));
            return this;
        }

        public RestApiRequest AddParameter<T>(string name, T value, bool encode = true) where T : struct {
            request.AddParameter(name, StringifyValue(value), encode);
            return this;
        }

        public RestApiRequest AddParameter(string name, object value, ParameterType type, bool encode = true) {
            if (type != ParameterType.RequestBody) {
                request.AddParameter(Parameter.CreateParameter(name, StringifyValue(value), type, encode));
                return this;
            }

            request.AddBody(request.RequestFormat == DataFormat.None ? StringifyValue(value) : value);
            return this;
        }

        private string StringifyValue(object value) {
            if (value is string sv) {
                return sv;
            }

            // Use serializer to format values instead of blind ToString()
            // TODO: use actual settings
            var serializer = new JsonNetSerializer();
            var s = serializer.Serialize(value);
            if (s.StartsWith('"')) {
                s = s.Substring(1, s.Length - 2);
            }

            return s;
        }

        public RestApiRequest AddStringBody(string body, DataFormat dataFormat) {
            ContentType contentType = ContentType.FromDataFormat(dataFormat);
            request.RequestFormat = dataFormat;
            request.AddParameter(new BodyParameter("", body, contentType));
            return this;
        }

        public RestApiRequest AddStringBody(string body, string contentType) {
            request.AddParameter(new BodyParameter("", body, contentType));
            return this;
        }

        public RestApiRequest AddJsonBody<T>(T obj, string contentType = "application/json") where T : class {
            request.RequestFormat = DataFormat.Json;
            if (obj is not string text) {
                request.AddParameter(new JsonParameter("", obj, contentType));
                return this;
            }

            request.AddStringBody(text, DataFormat.Json);
            return this;
        }

        public RestApiRequest AddXmlBody<T>(T obj, string contentType = "application/xml", string xmlNamespace = "") where T : class {
            request.RequestFormat = DataFormat.Xml;
            if (obj is not string text) {
                request.AddParameter(new XmlParameter("", obj, xmlNamespace, contentType));
                return this;
            }

            request.AddStringBody(text, DataFormat.Xml);
            return this;
        }

        public RestApiRequest AddQueryParameter(string name, string value, bool encode = true) {
            request.AddParameter(new QueryParameter(name, value, encode));
            return this;
        }

        public RestApiRequest AddQueryParameter<T>(string name, T value, bool encode = true) where T : struct {
            request.AddQueryParameter(name, value.ToString(), encode);
            return this;
        }
    }
}
