using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Hosting.Tests.Hosting {
    public class TestTimedHostedService : TimedHostedService, IMonitoredHostedService {
        public TestTimedHostedService(ILogger logger, TestTimedHostedConfiguration config) : base(logger, config.Enabled, config.Interval) {
        }

        public Task PublicExecuteAsync(CancellationToken stoppingToken) {
            return base.ExecuteAsync(stoppingToken);
        }

        protected override Task ExecuteIntervalAsync() {
            Executed = true;
            return Task.CompletedTask;
        }

        public bool Executed { get; set; } = false;
    }
}
