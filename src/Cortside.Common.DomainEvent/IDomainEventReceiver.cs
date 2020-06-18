using System;
using System.Collections.Generic;
using Amqp;

namespace Cortside.Common.DomainEvent {
    public delegate void ReceiverClosedCallback(IDomainEventReceiver receiver, DomainEventError error);
    public interface IDomainEventReceiver {
        event ReceiverClosedCallback Closed;
        void StartAndListen(IDictionary<string, Type> eventTypeLookup);
        void Close(TimeSpan? timeout = null);
        ReceiverLink Link { get; }
    }
}
