using Microsoft.AspNetCore.Http;

namespace Cortside.RestApiClient {
    public static class HttpContextUtility {
        public static string GetRequestIpAddress(HttpContext httpContext) {
            string ipList = httpContext?.Request?.Headers["X-FORWARDED-FOR"].ToString();
            if (!string.IsNullOrEmpty(ipList)) {
                return ipList.Split(',')[0];
            }

            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();
            if (httpContext!.Request?.Headers?.ContainsKey("REMOTE_ADDR") == true) {
                ip = httpContext.Request.Headers["REMOTE_ADDR"].ToString();
            }

            return ip;
        }
    }
}
