using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public delegate void PublisherClosedCallback(IDomainEventPublisher publisher, DomainEventError error);
    public interface IDomainEventPublisher {
        event PublisherClosedCallback Closed;
        Task SendAsync<T>(T @event) where T : class;
        DomainEventError Error { get; set; }
    }
}
