using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
        public Task Handle(TestEvent @event) {
            TestEvent.Instances.Add("none", @event);
            return Task.FromResult(0);
        }

        public Task Handle(DomainEventMessage<TestEvent> @event) {
            TestEvent.Instances.Add(@event.CorrelationId, @event.Data);
            return Task.FromResult(0);
        }
    }
}
