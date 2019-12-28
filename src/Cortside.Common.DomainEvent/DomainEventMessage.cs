namespace Cortside.Common.DomainEvent {
    public class DomainEventMessage<T> {
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public T Data { get; set; }
    }
}
