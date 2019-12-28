using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventPublisher : DomainEventComms, IDomainEventPublisher {
        public const string SCHEDULED_ENQUEUE_TIME_UTC = "x-opt-scheduled-enqueue-time";
   
        public event PublisherClosedCallback Closed;

        public DomainEventPublisher(ServiceBusPublisherSettings settings, ILogger<DomainEventComms> logger)
            : base(settings, logger) { }

        public DomainEventError Error { get; set; }

        public async Task SendAsync<T>(string eventType, string address, T @event) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await SendAsync(eventType, address, data, null);
        }

        public async Task SendAsync<T>(string eventType, string address, T @event, string correlationId) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await SendAsync(eventType, address, data, correlationId);
        }

        public async Task SendAsync<T>(T @event) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await SendAsync(eventType, address, data, null);
        }

        public async Task SendAsync<T>(T @event, string correlationId) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await SendAsync(eventType, address, data, correlationId);
        }

        public async Task SendAsync(string eventType, string address, string data) {
            await SendAsync(eventType, address, data, null);
        }

        public async Task SendAsync(string eventType, string address, string data, string correlationId) {
            var message = CreateMessage(eventType, data, correlationId);
            await InnerSendAsync(address, message);
        }

        public async Task ScheduleMessageAsync(string eventType, string address, string data, string correlationId, DateTime scheduledEnqueueTimeUtc) {
            var message = CreateMessage(eventType, data, correlationId);
            message.MessageAnnotations[SCHEDULED_ENQUEUE_TIME_UTC] = scheduledEnqueueTimeUtc;

            await InnerSendAsync(address, message);
        }

        private Message CreateMessage(string eventType, string data, string correlationId) {
            var messageId = Guid.NewGuid().ToString();
            var message = new Message(data) {
                Header = new Header {
                    Durable = (Settings.Durable == 2)
                },
                ApplicationProperties = new ApplicationProperties(),
                MessageAnnotations = new MessageAnnotations(),
                Properties = new Properties {
                    MessageId = messageId,
                    GroupId = eventType,
                    CorrelationId = correlationId
                }
            };
            message.ApplicationProperties[MESSAGE_TYPE_KEY] = eventType;
            return message;
        }

        public async Task InnerSendAsync(string address, Message message) {
            using (Logger.BeginScope(new Dictionary<string, object> {
                ["CorrelationId"] = message.Properties.CorrelationId,
                ["MessageId"] = message.Properties.MessageId,
                ["MessageType"] = message.Properties.GroupId
            })) {
                Logger.LogTrace($"Publishing message {message.Properties.MessageId} to {address} with body: {message.Body}");
                var session = CreateSession();
                var attach = new Attach() {
                    Target = new Target() { Address = address, Durable = Settings.Durable },
                    Source = new Source()
                };
                var sender = new SenderLink(session, Settings.AppName, attach, null);
                sender.Closed += OnClosed;

                try {
                    await sender.SendAsync(message);
                    Logger.LogInformation($"Published message {message.Properties.MessageId}");
                } finally {
                    if (sender.Error != null) {
                        Error = new DomainEventError();
                        Error.Condition = sender.Error.Condition.ToString();
                        Error.Description = sender.Error.Description;
                        Closed?.Invoke(this, Error);
                    }
                    if (!sender.IsClosed) {
                        await sender.CloseAsync(TimeSpan.FromSeconds(5));
                    }
                    await session.CloseAsync();
                    await session.Connection.CloseAsync();
                }
            }
        }

        private void OnClosed(IAmqpObject sender, Error error) {
            if (sender.Error != null) {
                Error = new DomainEventError();
                Error.Condition = sender.Error.Condition.ToString();
                Error.Description = sender.Error.Description;
            }
            Closed?.Invoke(this, Error);
        }

        public async Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await ScheduleMessageAsync(eventType, address, data, null, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await ScheduleMessageAsync(eventType, address, data, correlationId, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(string eventType, string address, T @event, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await ScheduleMessageAsync(eventType, address, data, null, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(string eventType, string address, T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await ScheduleMessageAsync(eventType, address, data, correlationId, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync(string eventType, string address, string data, DateTime scheduledEnqueueTimeUtc) {
            await ScheduleMessageAsync(eventType, address, data, null, scheduledEnqueueTimeUtc);
        }

        public async Task SendAsync<T>(T @event, MessageOptions options) where T : class {
            await SendAsync<T>(options.MessageType, options.Address, @event, options.CorrelationId);
        }

        public async Task SendAsync(string data, MessageOptions options) {
            throw new NotImplementedException();
        }

        public async Task ScheduleMessageAsync<T>(T @event, MessageOptions options, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public async Task ScheduleMessageAsync(string data, MessageOptions options, DateTime scheduledEnqueueTimeUtc) {
            throw new NotImplementedException();
        }
    }
}

