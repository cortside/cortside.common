using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
        public Task Handle(TestEvent @event) {
            TestEvent.Instance = @event;
            return Task.FromResult(0);
        }

        public Task Handle(DomainEventMessage<TestEvent> @event) {
            TestEvent.Instance = @event.Data;
            TestEvent.CorrelationId = @event.CorrelationId;
            return Task.FromResult(0);
        }

    }
}
