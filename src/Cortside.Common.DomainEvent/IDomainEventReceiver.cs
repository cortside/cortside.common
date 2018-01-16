using System;
using System.Collections.Generic;

namespace Cortside.Common.DomainEvent {
    public interface IDomainEventReceiver {
        void Receive(IDictionary<string, Type> eventTypeLookup);
        void Close(TimeSpan? timeout = null);
    }
}
