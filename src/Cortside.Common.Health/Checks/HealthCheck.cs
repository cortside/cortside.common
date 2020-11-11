using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cortside.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Health.Checks {
    public class HealthCheck : Check {
        private readonly BuildModel build;
        private readonly List<Check> checks;

        public HealthCheck(CheckConfiguration check, List<Check> checks, BuildModel build, IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(check, cache, logger, recorder) {
            this.build = build;
            this.checks = checks;
        }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var response = await Task.Run(() => new HealthModel() {
                Service = Name,
                Build = build,
                Checks = new Dictionary<string, ServiceStatusModel>(),
                Timestamp = DateTime.UtcNow,
                Uptime = GetRunningTime()
            });

            foreach (var check in checks.Where(c => c.Name != Name)) {
                response.Checks.Add(check.Name, check.Status);
            }

            response.Healthy = !response.Checks.Select(x => x.Value).Any(c => c != null && c.Required && !c.Healthy);
            response.Status = response.Healthy ? ServiceStatus.Ok : ServiceStatus.Failure;
            var degraded = response.Checks.Select(x => x.Value).Any(c => c != null && !c.Required && !c.Healthy);

            if (response.Healthy && degraded) {
                response.Status = ServiceStatus.Degraded;
            }

            if (response.Status != ServiceStatus.Ok) {
                logger.LogError($"Health check response for {Name} is unhealthy: {JsonConvert.SerializeObject(response)}");
            }

            return response;
        }

        private static TimeSpan GetRunningTime() {
            var runningTime = TimeSpan.MaxValue;
            try {
                Process currentProcess = Process.GetCurrentProcess();
                if (currentProcess != null) {
                    runningTime = DateTime.UtcNow.Subtract(currentProcess.StartTime);
                }
            } catch {
                // ignore this, not critical
            }

            return runningTime;
        }
    }
}
