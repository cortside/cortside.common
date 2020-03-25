using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.DomainEvent {
    /// <summary>
    /// Message receiver hosted service
    /// </summary>
    public class ReceiverHostedService : IHostedService, IDisposable {
        private readonly ILogger logger;
        private readonly IServiceProvider services;
        private readonly ReceiverHostedServiceSettings settings;
        private IDomainEventReceiver receiver;
        private System.Timers.Timer timer;
        private readonly object syncLock = new object();

        /// <summary>
        /// Message receiver hosted service
        /// </summary>
        public ReceiverHostedService(ILogger<ReceiverHostedService> logger, IServiceProvider services, ReceiverHostedServiceSettings settings) {
            this.logger = logger;
            this.services = services;
            this.settings = settings;
        }

        /// <summary>
        /// Interface method to start service
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken) {
            if (settings.Disabled) {
                logger.LogInformation("TimedServices are disabled");
            } else {
                while (!cancellationToken.IsCancellationRequested && settings.MessageTypes != null) {
                    if (receiver == null || receiver.Link == null || receiver.Link.IsClosed) {
                        lock (syncLock) {
                            logger.LogInformation("Receive Hosted Service is starting.");
                            // if there is a running timer, stop it
                            DisposeTimer();
                            // incase there was an instance that didn't get cleaned up
                            DisposeReceiver();
                            receiver = services.GetService<IDomainEventReceiver>();
                            if (receiver != null) {
                                logger.LogInformation("Starting receiver");
                                receiver.Closed -= OnReceiverClosed;
                                try {
                                    receiver.Receive(settings.MessageTypes);
                                    logger.LogInformation("Receiver started");
                                } catch (Exception e) {
                                    logger.LogCritical($"Unable to start receiver. \n {e}");
                                }

                                receiver.Closed += OnReceiverClosed;

                                timer = new System.Timers.Timer();
                                timer.Elapsed += OnTimedEvent;
                                timer.Interval = settings.TimedInterval;
                                timer.Enabled = true;

                            } else {
                                logger.LogError($"Found receiver was null");
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interface method to stop service
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Receiver Hosted Service is stopping.");
            DisposeTimer();
            DisposeReceiver();
            return Task.CompletedTask;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            try {
                // make sure no more events will happen
                DisposeTimer();
                logger.LogError($"Found receiver closed unexpectedly.");
                if (receiver != null && receiver.Link != null && receiver.Link.Error != null) {
                    var error = receiver?.Link?.Error;
                    if (error != null) {
                        logger.LogError($"Found receiver closed unexpectedly with error: {error.Condition} - {error.Description}");
                    }
                }
                DisposeReceiver();
                StartAsync(new CancellationToken(false));
            } catch (Exception ex) {
                logger.LogError(ex, $"Unhandled exception in OnTimedEvent event: {ex.Message}");
            }
        }
        private void OnReceiverClosed(IDomainEventReceiver receiver, DomainEventError error) {
            if (error == null) {
                logger.LogError("Handling OnReceiverClosed event with no error information");
            } else {
                logger.LogError($"Handling OnReceiverClosed event with error: {error.Condition} - {error.Description}");
            }
        }
        private void DisposeTimer() {
            if (timer != null) {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }
        private void DisposeReceiver() {
            receiver?.Close();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ReceiverHostedService() {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            DisposeReceiver();
            DisposeTimer();
        }

    }
}
