using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent {
    public interface IDomainEventPublisher {
	Task SendAsync<T>(T @event) where T : class;
    }
}
