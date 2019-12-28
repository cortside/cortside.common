using System;
using System.Collections.Generic;
using System.Text;

namespace Cortside.Common.DomainEvent {
    public class MessageOptions {
        public string MessageType { get; internal set; }
        public string Address { get; internal set; }
        public string CorrelationId { get; internal set; }
    }
}
