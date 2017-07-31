using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent {
    public interface IDomainEventHandler<T> where T : class
    {
	Task Handle(T @event);
    }
}
