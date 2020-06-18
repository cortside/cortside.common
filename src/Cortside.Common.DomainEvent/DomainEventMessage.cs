namespace Cortside.Common.DomainEvent {

    public class DomainEventMessage {
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public object Data { get; set; }
    }

    public class DomainEventMessage<T> : DomainEventMessage {
        public new T Data { get; set; }
    }
}
