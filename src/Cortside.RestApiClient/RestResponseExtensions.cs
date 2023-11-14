using System;
using Cortside.RestApiClient.MessageExceptions;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.RestApiClient {
    public static class RestResponseExtensions {
        /// <summary>
        /// Logs error and throws ExternalCommunicationFailureMessage exception with same formatted message
        /// </summary>
        /// <remarks>
        /// Uses String.Format, so template string uses positional values ({0}, {1}, etc)
        /// </remarks>
        /// <param name="response"></param>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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
