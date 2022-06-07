#pragma warning disable RCS1224 // Make method an extension method.

using System.Net;
using System.Reflection;
using RestSharp;

namespace Cortside.RestSharpClient.Tests {
    public static class RestResponseExtensions {
        public static RestResponse FromStatusCode(HttpStatusCode statusCode) {
            var response = new RestResponse();
            PropertyInfo propertyInfo = typeof(RestResponse).GetProperty(nameof(RestResponse.StatusCode));
            propertyInfo.SetValue(response, statusCode);

            return response;
        }
    }
}
