using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cortside.Common.Correlation;
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
        private readonly bool forceIntervalTimeout;

        /// <summary>
        /// Initializes new instance of the Hosted Service
        /// </summary>
        protected TimedHostedService(ILogger logger, bool enabled, int interval, bool generateCorrelationId = true, bool forceIntervalTimeout = true) {
            this.logger = logger;
            this.interval = interval;
            this.enabled = enabled;
            this.generateCorrelationId = generateCorrelationId;
            this.forceIntervalTimeout = forceIntervalTimeout;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // force async so that hosted service does not block Startup
#pragma warning disable _MissingConfigureAwait // Consider using .ConfigureAwait(false).
            await Task.Yield();
#pragma warning restore _MissingConfigureAwait // Consider using .ConfigureAwait(false).

            if (enabled) {
                logger.LogInformation($"{this.GetType().Name} is starting with interval of {interval} seconds");

                stoppingToken.Register(() => logger.LogDebug($"{this.GetType().Name} is stopping."));

                while (!stoppingToken.IsCancellationRequested) {
                    await IntervalAsync();
                    await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken).ConfigureAwait(false);
                }
                logger.LogInformation($"{this.GetType().Name} is stopping");
            } else {
                logger.LogInformation($"{this.GetType().Name} is disabled");
            }
        }

        private async Task IntervalAsync() {
            var cts = new CancellationTokenSource();

            try {
                if (forceIntervalTimeout) {
                    cts.CancelAfter(Convert.ToInt32(TimeSpan.FromSeconds(interval).TotalMilliseconds));
                }

                var correlationId = CorrelationContext.GetCorrelationId(generateCorrelationId);
                using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId })) {
                    logger.LogDebug($"{this.GetType().Name} is working");

                    await ExecuteIntervalAsync().ConfigureAwait(false);
                }
            } catch (OperationCanceledException) {
                logger.LogWarning($"{this.GetType().Name} is taking longer than {interval} seconds");
            } catch (Exception ex) {
                logger.LogError(ex, this.GetType().Name);
            } finally {
                cts.Dispose();
            }
        }

        protected abstract Task ExecuteIntervalAsync();
    }
}
