using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventPublisher : DomainEventComms, IDomainEventPublisher {
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
            var messageId = Guid.NewGuid().ToString();
            using (Logger.BeginScope(new Dictionary<string, object> {
                ["CorrelationId"] = correlationId,
                ["MessageId"] = messageId,
                ["MessageType"] = eventType
            })) {
                var session = CreateSession();
                var attach = new Attach() {
                    Target = new Target() { Address = address, Durable = Settings.Durable },
                    Source = new Source()
                };
                var sender = new SenderLink(session, Settings.AppName, attach, null);
                sender.Closed += OnClosed;
                var message = new Message(data) {
                    Header = new Header {
                        Durable = (Settings.Durable == 2)
                    },
                    ApplicationProperties = new ApplicationProperties(),
                    Properties = new Properties {
                        MessageId = messageId,
                        GroupId = eventType,
                        CorrelationId = correlationId
                    }
                };
                message.ApplicationProperties[MESSAGE_TYPE_KEY] = eventType;

                try {
                    await sender.SendAsync(message);
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
