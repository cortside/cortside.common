using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public interface IDomainEventMessageHandler<T> where T : class {
        Task<HandlerResult> HandleAsync(DomainEventMessage<T> @event);
    }
}
