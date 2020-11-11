using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace Cortside.Health.Recorders {
    public class ApplicationInsightsRecorder : IAvailabilityRecorder {

        private readonly ILogger<ApplicationInsightsRecorder> logger;
        private readonly TelemetryClient telemetryClient;

        public ApplicationInsightsRecorder(ILogger<ApplicationInsightsRecorder> logger, TelemetryClient telemetryClient) {
            this.logger = logger;
            this.telemetryClient = telemetryClient;
        }

        public void RecordAvailability(string service, TimeSpan duration, bool healthy, string message) {
            logger.LogInformation($"Executing availability test run for {service} at: {DateTime.UtcNow}");

            var telemetry = new AvailabilityTelemetry {
                Id = Guid.NewGuid().ToString("N"),
                Name = service,
                RunLocation = Environment.MachineName,
                Success = false
            };

            if (healthy) {
                telemetry.Success = true;
            } else {
                telemetry.Message = message;

                var exceptionTelemetry = new ExceptionTelemetry();
                exceptionTelemetry.Context.Operation.Id = telemetry.Id;
                exceptionTelemetry.Properties.Add("TestName", telemetry.Name);
                exceptionTelemetry.Properties.Add("TestLocation", telemetry.RunLocation);
                telemetryClient.TrackException(exceptionTelemetry);
            }

            telemetry.Duration = duration;
            telemetry.Timestamp = DateTimeOffset.UtcNow;

            telemetryClient.TrackAvailability(telemetry);
            // call flush to ensure telemetry is sent
            telemetryClient.Flush();
        }
    }
}
