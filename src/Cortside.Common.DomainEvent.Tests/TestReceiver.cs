using System;
using System.Collections.Generic;
using Amqp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestReceiver : DomainEventReceiver {

        public TestReceiver(ServiceBusReceiverSettings settings, IServiceProvider provider, ILogger<DomainEventComms> logger) : base(settings, provider, logger) { }

        public void Setup(IDictionary<string, Type> eventTypeLookup) {
            EventTypeLookup = eventTypeLookup;
        }

        public void MessageCallback(IReceiverLink receiver, Message message) {
            base.OnMessageCallback(receiver, message);
        }

        internal void SetProvider(ServiceProvider provider) {
            base.Provider = provider;
        }
    }
}
