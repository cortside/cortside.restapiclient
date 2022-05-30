using System;

namespace Cortside.RestSharpClient.Tests {
    public class CustomException : Exception {
        public CustomException() : base() {
        }

        public CustomException(string message) : base(message) {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
