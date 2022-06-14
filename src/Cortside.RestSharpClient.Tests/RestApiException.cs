using System;
using System.Runtime.Serialization;

namespace Cortside.RestSharpClient.Tests.Clients {
    [Serializable]
    internal class RestApiException : Exception {
        public RestApiException() {
        }

        public RestApiException(string message) : base(message) {
        }

        public RestApiException(string message, Exception innerException) : base(message, innerException) {
        }

        protected RestApiException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}