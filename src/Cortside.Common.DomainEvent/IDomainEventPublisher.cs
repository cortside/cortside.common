using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public delegate void PublisherClosedCallback(IDomainEventPublisher publisher, DomainEventError error);
    public interface IDomainEventPublisher {
        event PublisherClosedCallback Closed;
        Task SendAsync<T>(string eventType, string address, T @event) where T : class;
        Task SendAsync<T>(T @event) where T : class;
        Task SendAsync(string eventType, string address, string data);
        DomainEventError Error { get; set; }
    }
}
