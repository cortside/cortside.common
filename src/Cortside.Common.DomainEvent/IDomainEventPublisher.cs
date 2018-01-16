using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public interface IDomainEventPublisher {
        Task SendAsync<T>(T @event) where T : class;
    }
}
