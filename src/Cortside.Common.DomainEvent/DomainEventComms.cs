using Amqp;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.DomainEvent {
    public abstract class DomainEventComms {
        public const string MESSAGE_TYPE_KEY = "Message.Type.FullName";
        public const string SCHEDULED_ENQUEUE_TIME_UTC = "x-opt-scheduled-enqueue-time";

        protected ServiceBusSettings Settings { get; }

        protected ILogger<DomainEventComms> Logger { get; }

        protected DomainEventComms(ServiceBusSettings settings, ILogger<DomainEventComms> logger) {
            Settings = settings;
            Logger = logger;
        }

        protected virtual Session CreateSession() {
            var connStr = $"{Settings.Protocol}://{Settings.PolicyName}:{Settings.Key}@{Settings.Namespace}/";
            var conn = new Connection(new Address(connStr));
            return new Session(conn);
        }
    }
}
