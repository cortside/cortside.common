using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Hosting {
    /// <summary>
    /// Base timed hosted service
    /// </summary>
    public abstract class TimedHostedService : BackgroundService {
        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        protected readonly ILogger logger;
        private readonly int interval;
        private readonly bool enabled;

        /// <summary>
        /// Initializes new instance of the Hosted Service
        /// </summary>
        protected TimedHostedService(ILogger logger, bool enabled, int interval) {
            this.logger = logger;
            this.interval = interval;
            this.enabled = enabled;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            if (enabled) {
                logger.LogInformation($"{this.GetType().Name} is starting with interval of {interval} seconds");

                stoppingToken.Register(() => logger.LogDebug($"{this.GetType().Name} is stopping."));

                while (!stoppingToken.IsCancellationRequested) {
                    await IntervalAsync();
                    await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
                }
                logger.LogInformation($"{this.GetType().Name} is stopping");
            } else {
                logger.LogInformation($"{this.GetType().Name} is disabled");
            }
        }

        private async Task IntervalAsync() {
            //TODO: create correlationId and set in logging context
            logger.LogDebug($"{this.GetType().Name} is working");
            await semaphore.WaitAsync();

            try {
                await ExecuteIntervalAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                logger.LogError(ex, this.GetType().Name);
            } finally {
                semaphore.Release();
            }
        }

        protected abstract Task ExecuteIntervalAsync();
    }
}
