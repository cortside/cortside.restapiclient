using System;
using Cortside.RestApiClient.MessageExceptions;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.RestApiClient {
    public static class RestResponseExtensions {
        public static Exception LoggedFailureException(this RestResponse response, ILogger logger, string message, params object[] args) {
            if (response.ErrorException != null) {
                logger.LogError(response.ErrorException, message, args);
                return new ExternalCommunicationFailureMessage(string.Format(message, args), response.ErrorException);
            }

            logger.LogError(message, args);
            return new ExternalCommunicationFailureMessage(string.Format(message, args));
        }
    }
}
