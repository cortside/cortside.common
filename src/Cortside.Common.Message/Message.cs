using System;

namespace Cortside.Common.Message {

    public class Message : IMessage {

        private Message() {
        }

        public Message(String routingKeyPrefix) {
            this.MessageId = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.UtcNow;
            this.MessageType = this.GetType().FullName;
            this.RoutingKey = routingKeyPrefix + "." + this.GetType().Name;
            this.SourceMessageId = null;
        }

        // TODO: making these public for now because of desrialization
        public String MessageId { get; set; }

        public DateTime Timestamp { get; set; }
        public String MessageType { get; set; }
        public String RoutingKey { get; set; }

        /// <summary>
        /// Reference to the source message, if any, that published this message. For message chaining.
        /// </summary>
        public String SourceMessageId { get; set; }
    }
}
