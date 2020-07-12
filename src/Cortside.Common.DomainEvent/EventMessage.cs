using Amqp;
using Amqp.Types;

namespace Cortside.Common.DomainEvent {
    public class EventMessage {
        private readonly Message message;
        private readonly ReceiverLink link;
        private readonly DomainEventMessage domainEvent;

        internal EventMessage(DomainEventMessage domainEvent, Message message, ReceiverLink link) {
            this.message = message;
            this.link = link;
            this.domainEvent = domainEvent;
        }

        public DomainEventMessage Message => domainEvent;

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

        public T GetData<T>() {
            return (T)domainEvent.Data;
        }
    }
}
