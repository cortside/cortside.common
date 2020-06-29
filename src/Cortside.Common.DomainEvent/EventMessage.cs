using Amqp;
using Amqp.Types;

namespace Cortside.Common.DomainEvent {
    public class EventMessage : DomainEventMessage {
        private readonly Message message;
        private readonly ReceiverLink link;

        internal EventMessage(Message message, ReceiverLink link) {
            this.message = message;
            this.link = link;

            base.MessageId = message.Properties.MessageId;
            base.CorrelationId = message.Properties.CorrelationId;
            base.MessageTypeName = message.ApplicationProperties[DomainEventComms.MESSAGE_TYPE_KEY] as string;
        }

        public DomainEventMessage Message { get; }

        public void Reject() {
            link.Reject(message);
        }

        public void Release() {
            link.Release(message);
        }

        public void Accept() {
            link.Accept(message);
        }

        public void Modify(bool deliveryFailed, bool undeliverableHere = false, Fields messageAnnotations = null) {
            link.Modify(message, deliveryFailed, undeliverableHere, messageAnnotations);
        }
    }
}
