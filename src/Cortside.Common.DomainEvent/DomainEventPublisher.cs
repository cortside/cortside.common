using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventPublisher : DomainEventComms, IDomainEventPublisher {
        public event PublisherClosedCallback Closed;

        public DomainEventPublisher(ServiceBusSettings settings, ILogger<DomainEventComms> logger)
            : base(settings, logger) { }

        public DomainEventError Error { get; set; }

        public async Task SendAsync<T>(T @event) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;

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
                    MessageId = Guid.NewGuid().ToString(),
                    GroupId = eventType
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
