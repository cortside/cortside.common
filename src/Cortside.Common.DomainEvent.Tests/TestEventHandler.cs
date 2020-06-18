using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
        public Task Handle(DomainEventMessage<TestEvent> @event) {
            TestEvent.Instances.Add(@event.CorrelationId, @event.Data);
            return Task.FromResult(0);
        }
    }
}
