using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent {
    public interface IDomainEventReceiver {
	void Receive(IDictionary<string, Type> eventTypeLookup);
	void Close(TimeSpan? timeout = null);
    }
}
