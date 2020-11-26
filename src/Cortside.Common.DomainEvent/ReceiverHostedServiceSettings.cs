using System;
using System.Collections.Generic;
using System.Text;

namespace Cortside.Common.DomainEvent {
    /// <summary>
    /// Settings for the ReceiverHostedService
    /// </summary>
    public class ReceiverHostedServiceSettings {
        /// <summary>
        /// Message types for the Domain Event Receiver to handle
        /// </summary>
        public Dictionary<string, Type> MessageTypes { get; set; }

        /// <summary>
        /// Controls whether receiver hosted service is enabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Frequency which the receiver attempts to connect to the message broker
        /// </summary>
        public int TimedInterval { get; set; }
    }
}
