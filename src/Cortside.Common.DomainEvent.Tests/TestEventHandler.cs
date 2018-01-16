using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
        public Task Handle(TestEvent @event) {
            TestEvent.Instance = @event;
            return Task.FromResult(0);
        }
    }
}
