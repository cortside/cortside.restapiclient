#pragma warning disable RCS1085 // Use auto-implemented property.
#pragma warning disable IDE0032 // Use auto-implemented property.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestSharpClient {
    public class RestApiClientOptions {
        private readonly RestClientOptions rcOptions = new RestClientOptions() {
            AutomaticDecompression = DecompressionMethods.GZip,
            FollowRedirects = true,
            UserAgent = DefaultUserAgent,
            Encoding = Encoding.UTF8,
            FailOnDeserializationError = true
        };

        public RestApiClientOptions() {
            Serializer = new JsonNetSerializer();
            Policy = Polly.Policy.NoOpAsync<RestResponse>();
            Cache = new NullDistributedCache();
        }

        public RestApiClientOptions(Uri baseUrl) {
            rcOptions.BaseUrl = baseUrl;
            Serializer = new JsonNetSerializer();
            Policy = Polly.Policy.NoOpAsync<RestResponse>();
            Cache = new NullDistributedCache();
        }

        public RestApiClientOptions(string baseUrl) : this(new Uri(baseUrl)) { }

        private RestApiClientOptions(RestClientOptions rcoptions) {
            rcOptions = rcoptions;
        }

        public static RestApiClientOptions From(RestClientOptions rcoptions) {
            return new RestApiClientOptions(rcoptions);
        }

        public RestClientOptions Options => rcOptions;

        static readonly Version Version = new AssemblyName(typeof(RestApiClientOptions).Assembly.FullName!).Version!;
        static readonly string DefaultUserAgent = $"RestApiClient/{Version}";

        public IAuthenticator Authenticator { get; set; }

        public IRestSerializer Serializer { get; set; }

        public IDistributedCache Cache { get; set; }

        public IAsyncPolicy<RestResponse> Policy { get; set; }

        /// <summary>
        /// Explicit Host header value to use in requests independent from the request URI.
        /// If null, default host value extracted from URI is used.
        /// </summary>
        public Uri BaseUrl {
            get { return rcOptions.BaseUrl; }
            set { rcOptions.BaseUrl = value; }
        }

        public Func<HttpMessageHandler, HttpMessageHandler> ConfigureMessageHandler {
            get { return rcOptions.ConfigureMessageHandler; }
            set { rcOptions.ConfigureMessageHandler = value; }
        }

        /// <summary>
        /// In general you would not need to set this directly. Used by the NtlmAuthenticator.
        /// </summary>
        public ICredentials Credentials {
            get { return rcOptions.Credentials; }
            set { rcOptions.Credentials = value; }
        }

        /// <summary>
        /// Determine whether or not the "default credentials" (e.g. the user account under which the current process is
        /// running) will be sent along to the server. The default is false.
        /// </summary>
        public bool UseDefaultCredentials {
            get { return rcOptions.UseDefaultCredentials; }
            set { rcOptions.UseDefaultCredentials = value; }
        }

        /// <summary>
        /// Set to true if you need the Content-Type not to have the charset
        /// </summary>
        public bool DisableCharset {
            get { return rcOptions.DisableCharset; }
            set { rcOptions.DisableCharset = value; }
        }

        public DecompressionMethods AutomaticDecompression {
            get { return rcOptions.AutomaticDecompression; }
            set { rcOptions.AutomaticDecompression = value; }
        }

        public int? MaxRedirects {
            get { return rcOptions.MaxRedirects; }
            set { rcOptions.MaxRedirects = value; }
        }

        /// <summary>
        /// X509CertificateCollection to be sent with request
        /// </summary>
        public X509CertificateCollection ClientCertificates {
            get { return rcOptions.ClientCertificates; }
            set { rcOptions.ClientCertificates = value; }
        }

        public IWebProxy Proxy {
            get { return rcOptions.Proxy; }
            set { rcOptions.Proxy = value; }
        }

        public CacheControlHeaderValue CachePolicy {
            get { return rcOptions.CachePolicy; }
            set { rcOptions.CachePolicy = value; }
        }

        public bool FollowRedirects {
            get { return rcOptions.FollowRedirects; }
            set { rcOptions.FollowRedirects = value; }
        }

        public CookieContainer CookieContainer {
            get { return rcOptions.CookieContainer; }
            set { rcOptions.CookieContainer = value; }
        }

        public string UserAgent {
            get => rcOptions.UserAgent;
            set => rcOptions.UserAgent = value;
        }

        public Encoding Encoding {
            get => rcOptions.Encoding;
            set => rcOptions.Encoding = value;
        }

        public int MaxTimeout {
            get => rcOptions.MaxTimeout;
            set => rcOptions.MaxTimeout = value;
        }

        /// <summary>
        /// Flag to send authorisation header with the HttpWebRequest
        /// </summary>
        public bool PreAuthenticate {
            get => rcOptions.PreAuthenticate;
            set => rcOptions.PreAuthenticate = value;
        }

        /// <summary>
        /// Modifies the default behavior of RestSharp to swallow exceptions.
        /// When set to <c>true</c>, a <see cref="DeserializationException"/> will be thrown
        /// in case RestSharp fails to deserialize the response.
        /// </summary>
        public bool ThrowOnDeserializationError {
            get => rcOptions.ThrowOnDeserializationError;
            set => rcOptions.ThrowOnDeserializationError = value;
        }

        /// <summary>
        /// Modifies the default behavior of RestSharp to swallow exceptions.
        /// When set to <c>true</c>, RestSharp will consider the request as unsuccessful
        /// in case it fails to deserialize the response.
        /// </summary>
        public bool FailOnDeserializationError {
            get => rcOptions.FailOnDeserializationError;
            set => rcOptions.FailOnDeserializationError = value;
        }

        /// <summary>
        /// Modifies the default behavior of RestSharp to swallow exceptions.
        /// When set to <c>true</c>, exceptions will be re-thrown.
        /// </summary>
        public bool ThrowOnAnyError {
            get => rcOptions.ThrowOnAnyError;
            set => rcOptions.ThrowOnAnyError = value;
        }

        /// <summary>
        /// Callback function for handling the validation of remote certificates. Useful for certificate pinning and
        /// overriding certificate errors in the scope of a request.
        /// </summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback {
            get => rcOptions.RemoteCertificateValidationCallback;
            set => rcOptions.RemoteCertificateValidationCallback = value;
        }

        public string BaseHost {
            get => rcOptions.BaseHost;
            set => rcOptions.BaseHost = value;
        }

        /// <summary>
        /// By default, RestSharp doesn't allow multiple parameters to have the same name.
        /// This properly allows to override the default behavior.
        /// </summary>
        public bool AllowMultipleDefaultParametersWithSameName {
            get => rcOptions.AllowMultipleDefaultParametersWithSameName;
            set => rcOptions.AllowMultipleDefaultParametersWithSameName = value;
        }
    }
}
