using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Amqp.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventPublisher : DomainEventComms, IDomainEventPublisher {
        public event PublisherClosedCallback Closed;

        public DomainEventPublisher(ServiceBusPublisherSettings settings, ILogger<DomainEventComms> logger)
            : base(settings, logger) { }

        public DomainEventError Error { get; set; }

        public async Task SendAsync<T>(T @event) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await SendAsync(eventType, address, data, null, null);
        }

        public async Task SendAsync<T>(T @event, string correlationId) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await SendAsync(eventType, address, data, correlationId, null);
        }

        public async Task SendAsync<T>(T @event, string correlationId, string messageId) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await SendAsync(eventType, address, data, correlationId, messageId);
        }

        public async Task SendAsync<T>(T @event, string eventType, string address, string correlationId) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await SendAsync(eventType, address, data, correlationId, null);
        }

        public async Task SendAsync(string eventType, string address, string data, string correlationId, string messageId) {
            var message = CreateMessage(eventType, data, correlationId, messageId);
            await InnerSendAsync(address, message);
        }

        public async Task ScheduleMessageAsync(string data, string eventType, string address, string correlationId, string messageId, DateTime scheduledEnqueueTimeUtc) {
            var message = CreateMessage(eventType, data, correlationId, messageId);
            message.MessageAnnotations[new Symbol(SCHEDULED_ENQUEUE_TIME_UTC)] = scheduledEnqueueTimeUtc;

            await InnerSendAsync(address, message);
        }

        public async Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await ScheduleMessageAsync(data, eventType, address, null, null, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await ScheduleMessageAsync(data, eventType, address, correlationId, null, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(T @event, string correlationId, string messageId, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;
            await ScheduleMessageAsync(data, eventType, address, correlationId, messageId, scheduledEnqueueTimeUtc);
        }

        public async Task ScheduleMessageAsync<T>(T @event, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            await ScheduleMessageAsync(data, eventType, address, correlationId, null, scheduledEnqueueTimeUtc);
        }

        private Message CreateMessage(string eventType, string data, string correlationId, string messageId) {
            var messageIdentifier = messageId ?? Guid.NewGuid().ToString();
            var message = new Message(data) {
                Header = new Header {
                    Durable = (Settings.Durable == 2)
                },
                ApplicationProperties = new ApplicationProperties(),
                MessageAnnotations = new MessageAnnotations(),
                Properties = new Properties {
                    MessageId = messageIdentifier,
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
    }
}
