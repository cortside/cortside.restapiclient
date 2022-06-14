//using System;
//using System.Collections.Generic;
//using System.Net;
//using RestSharp;

//namespace Cortside.RestSharpClient {
//    public interface IRestApiResponse {
//        string Content { get; set; }
//        ICollection<string> ContentEncoding { get; set; }
//        IReadOnlyCollection<HeaderParameter> ContentHeaders { get; set; }
//        long? ContentLength { get; set; }
//        string ContentType { get; set; }
//        CookieCollection Cookies { get; set; }
//        Exception ErrorException { get; set; }
//        string ErrorMessage { get; set; }
//        IReadOnlyCollection<HeaderParameter> Headers { get; set; }
//        bool IsSuccessful { get; set; }
//        byte[] RawBytes { get; set; }
//        RestRequest Request { get; set; }
//        ResponseStatus ResponseStatus { get; set; }
//        Uri ResponseUri { get; set; }
//        string RootElement { get; set; }
//        string Server { get; set; }
//        HttpStatusCode StatusCode { get; set; }
//        string StatusDescription { get; set; }
//        Version Version { get; set; }
//    }
//}
