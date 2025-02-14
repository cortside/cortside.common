using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Hosting.Tests {
    public class TestTimedHostedService : TimedHostedService {
        private readonly int delay;

        public TestTimedHostedService(ILogger logger, bool enabled, int interval, int delay = 0) : base(logger, enabled, interval, true, true) {
            this.delay = delay;
        }

        public Task PublicExecuteAsync(CancellationToken stoppingToken) {
            return ExecuteAsync(stoppingToken);
        }

        protected override async Task ExecuteIntervalAsync() {
            try {
                if (delay > 0) {
                    await Task.Delay(delay);
                }

                Executed = true;
            } catch (OperationCanceledException ex) {

            }
        }

        public bool Executed { get; set; } = false;
    }
}
