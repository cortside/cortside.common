using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages {
    [Serializable]
    public class MessageListException : MessageException {
        public List<MessageException> Messages { get; protected set; }

        public MessageListException() : base("One or more error occurred.") {
            Messages = new List<MessageException>();
        }

        public MessageListException(string message) : base(message) {
            Messages = new List<MessageException>();
        }

        public MessageListException(params MessageException[] messages) : this(messages.ToList()) { }

        public MessageListException(IEnumerable<MessageException> messages) {
            Messages = new List<MessageException>();
            Messages.AddRange(messages);
        }

        protected MessageListException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected MessageListException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected MessageListException(string message, string property) : base(message, property) {
        }

        protected MessageListException(string message, Exception innerException) : base(message, innerException) {
        }

        public bool HasMessageOfType<T>() {
            return Messages.Any(m => m is T);
        }
    }
}
