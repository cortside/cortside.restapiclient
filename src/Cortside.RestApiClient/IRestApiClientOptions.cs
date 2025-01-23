using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Cortside.RestApiClient {
    public interface IRestApiClientOptions {
        bool AllowMultipleDefaultParametersWithSameName { get; set; }
        IAuthenticator Authenticator { get; set; }
        DecompressionMethods AutomaticDecompression { get; set; }
        string BaseHost { get; set; }
        Uri BaseUrl { get; set; }
        IDistributedCache Cache { get; set; }
        CacheControlHeaderValue CachePolicy { get; set; }
        X509CertificateCollection ClientCertificates { get; set; }
        Func<HttpMessageHandler, HttpMessageHandler> ConfigureMessageHandler { get; set; }
        CookieContainer CookieContainer { get; set; }
        ICredentials Credentials { get; set; }
        bool DisableCharset { get; set; }
        Encoding Encoding { get; set; }
        bool FailOnDeserializationError { get; set; }
        bool FollowRedirects { get; set; }
        int? MaxRedirects { get; set; }
        TimeSpan? Timeout { get; set; }
        RestClientOptions Options { get; }
        IAsyncPolicy<RestResponse> Policy { get; set; }
        bool PreAuthenticate { get; set; }
        IWebProxy Proxy { get; set; }
        RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }
        IRestSerializer Serializer { get; set; }
        bool ThrowOnAnyError { get; set; }
        bool ThrowOnDeserializationError { get; set; }
        bool UseDefaultCredentials { get; set; }
        string UserAgent { get; set; }
        bool EnableForwardHeaders { get; set; }
    }
}
