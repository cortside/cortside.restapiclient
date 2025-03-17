#pragma warning disable SYSLIB0051 // XmlReader.Create() is obsolete: 'Use XmlReader.Create() method using XmlReaderSettings instead.'

using System;
using System.Runtime.Serialization;

namespace Cortside.RestApiClient {
    [Serializable]
    public class RestApiException : Exception {
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
