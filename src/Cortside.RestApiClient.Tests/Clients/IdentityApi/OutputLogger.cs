using Microsoft.Extensions.Logging;

namespace Cortside.RestApiClient.Tests.Clients.IdentityApi {
    public class OutputLogger : HttpTracer.Logger.ILogger {
        private readonly ILogger logger;

        public OutputLogger(ILogger logger) {
            this.logger = logger;
        }

        public void Log(string message) {
            logger.LogInformation(message);
        }
    }
}
