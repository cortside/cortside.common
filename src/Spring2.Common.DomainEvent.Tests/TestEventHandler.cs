using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent.Tests {
    public class TestEventHandler : IDomainEventHandler<TestEvent> {
	public Task Handle(TestEvent @event) {
	    TestEvent.Instance = @event;
	    return Task.FromResult(0);
	}
    }
}
