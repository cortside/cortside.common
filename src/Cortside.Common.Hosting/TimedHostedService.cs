using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Hosting {
    /// <summary>
    /// Base timed hosted service
    /// </summary>
    public abstract class TimedHostedService : BackgroundService {
        protected readonly ILogger logger;
        private readonly int interval;
        private readonly bool enabled;
        private readonly bool generateCorrelationId;
        private static readonly AsyncLocal<string> CorrelationId = new AsyncLocal<string>();

        /// <summary>
        /// Initializes new instance of the Hosted Service
        /// </summary>
        protected TimedHostedService(ILogger logger, bool enabled, int interval, bool generateCorrelationId = true) {
            this.logger = logger;
            this.interval = interval;
            this.enabled = enabled;
            this.generateCorrelationId = generateCorrelationId;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // force async so that hosted service does not block Startup
#pragma warning disable _MissingConfigureAwait // Consider using .ConfigureAwait(false).
            await Task.Yield();
#pragma warning restore _MissingConfigureAwait // Consider using .ConfigureAwait(false).

            if (enabled) {
                logger.LogInformation($"{GetType().Name} is starting with interval of {interval} seconds");

                stoppingToken.Register(() => logger.LogDebug($"{GetType().Name} is stopping."));

                while (!stoppingToken.IsCancellationRequested) {
                    await IntervalAsync();
                    await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken).ConfigureAwait(false);
                }
                logger.LogInformation($"{GetType().Name} is stopping");
            } else {
                logger.LogInformation($"{GetType().Name} is disabled");
            }
        }

        private async Task IntervalAsync() {
            var correlationId = GetCorrelationId(generateCorrelationId);
            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId })) {
                logger.LogDebug($"{GetType().Name} is working");

                try {
                    await ExecuteIntervalAsync().ConfigureAwait(false);
                } catch (Exception ex) {
                    logger.LogError(ex, GetType().Name);
                }
            }
        }

        protected abstract Task ExecuteIntervalAsync();

        public static string GetCorrelationId(bool generateCorrelationId = true) {
            var correlationId = CorrelationId.Value;
            if (generateCorrelationId && string.IsNullOrWhiteSpace(correlationId)) {
                CorrelationId.Value = Guid.NewGuid().ToString();
            }
            return CorrelationId.Value;
        }
    }
}
