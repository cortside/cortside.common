using Amqp;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.DomainEvent {
    public abstract class DomainEventComms {
        protected const string MESSAGE_TYPE_KEY = "Message.Type.FullName";
        public ServiceBusSettings Settings { get; }

        protected ILogger<DomainEventComms> Logger { get; }

        public DomainEventComms(ServiceBusSettings settings, ILogger<DomainEventComms> logger) {
            Settings = settings;
            Logger = logger;
        }

        protected virtual Session CreateSession() {
            var connStr = $"{Settings.Protocol}://{Settings.PolicyName}:{Settings.Key}@{Settings.Namespace}/";
            var conn = new Connection(new Address(connStr));
            var session = new Session(conn);
            return session;
        }
    }
}
