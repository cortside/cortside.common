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
    }
}
