using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventPublisher : DomainEventComms, IDomainEventPublisher {

        public DomainEventPublisher(ServiceBusSettings settings, ILogger<DomainEventComms> logger)
            : base(settings, logger) { }

        public async Task SendAsync<T>(T @event) where T : class {
            var data = JsonConvert.SerializeObject(@event);
            var eventType = @event.GetType().FullName;
            var address = Settings.Address + @event.GetType().Name;

            var session = CreateSession();
            var sender = new SenderLink(session, Settings.AppName, address);
            var message = new Message(data) {
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
                await sender.CloseAsync(TimeSpan.Zero);
            }
        }
    }
}
