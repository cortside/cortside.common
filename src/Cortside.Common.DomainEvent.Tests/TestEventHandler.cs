using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
        public async Task<HandlerResult> HandleAsync(DomainEventMessage<TestEvent> @event) {
            TestEvent.Instances.Add(@event.CorrelationId, @event.Data);

            if (@event.Data.IntValue == int.MinValue) {
                var x = 0;
                _ = 1 / x;
            }

            if (@event.Data.IntValue > 0) {
                return await Task.FromResult(HandlerResult.Success);
            } else if (@event.Data.IntValue == 0) {
                return await Task.FromResult(HandlerResult.Retry);
            } else {
                return await Task.FromResult(HandlerResult.Failed);
            }
        }
    }
}
