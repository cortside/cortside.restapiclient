#pragma warning disable RCS1224 // Make method an extension method.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Cortside.RestApiClient.Tests {
    public static class HttpContextAccessorExtensions {

        public static IHttpContextAccessor SetHttpContext(this IHttpContextAccessor accessor, string host, string requestPath, KeyValuePair<string, StringValues> header) {
            var context = new DefaultHttpContext {
                Request = {
                    Path = requestPath,
                    Host = new HostString(host),
                    Headers = {  }
                }
            };

            if (!string.IsNullOrWhiteSpace(header.Value)) {
                context.Request.Headers.Append(header.Key, header.Value);
            }

            accessor.HttpContext = context;
            return accessor;
        }
    }
}
