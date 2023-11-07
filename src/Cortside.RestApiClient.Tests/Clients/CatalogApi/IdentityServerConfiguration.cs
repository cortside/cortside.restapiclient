// NOTE: this file is a duplicate from Cortside.AspNetCore.AccessControl for testing purposes

using System;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace Cortside.AspNetCore.AccessControl {
    public class IdentityServerConfiguration {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public bool EnableCaching { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
