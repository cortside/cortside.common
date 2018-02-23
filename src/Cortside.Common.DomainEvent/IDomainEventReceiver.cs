using System;
using System.Collections.Generic;

namespace Cortside.Common.DomainEvent {
    public delegate void ClosedCallback(IDomainEventReceiver receiver, DomainEventError error);
    public interface IDomainEventReceiver {
        event ClosedCallback Closed;
        void Receive(IDictionary<string, Type> eventTypeLookup);
        void Close(TimeSpan? timeout = null);
    }
}
