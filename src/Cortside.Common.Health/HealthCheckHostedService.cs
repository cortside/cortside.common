using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.Common.Health.Checks;
using Cortside.Common.Health.Models;
using Cortside.Common.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Health {

    /// <summary>
    /// Background service for health check
    /// </summary>
    public class HealthCheckHostedService : TimedHostedService {

        private readonly List<Check> checks;

        public HealthCheckHostedService(ILogger<HealthCheckHostedService> logger, HealthCheckServiceConfiguration config, IMemoryCache cache, IConfiguration configuration, ICheckFactory factory, BuildModel build) : base(logger, config.Enabled, config.Interval) {
            config.Name = factory.ExpandTemplate(config.Name);
            checks = new List<Check>();
            foreach (var check in config.Checks) {
                checks.Add(factory.Create(check));
            }
            checks.Add(new HealthCheck(new CheckConfiguration() { Name = config.Name, CacheDuration = config.CacheDuration }, checks, build, cache, factory.Logger, factory.Recorder));
        }

        protected override async Task ExecuteIntervalAsync() {
            var tasks = checks.Select(t => t.InternalExecuteAsync()).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
