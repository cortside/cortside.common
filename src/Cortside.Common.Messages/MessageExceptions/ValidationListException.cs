using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class ValidationListException : MessageListException {
        public ValidationListException() { }

        public ValidationListException(string message) : base(message) { }

        public ValidationListException(string message, System.Exception innerException) : base(message, innerException) { }

        protected ValidationListException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ValidationListException(params MessageException[] messages) : base(messages) { }

        public ValidationListException(IEnumerable<MessageException> messages) : this() {
            Messages = messages.ToList();
        }
    }
}
