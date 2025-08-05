using System.Collections.Generic;
using System.Linq;

namespace Cortside.Common.Messages.MessageExceptions {
    public class ValidationListException : MessageListException {
        public ValidationListException() : base("Validation failed") {
            Messages = [];
        }

        public ValidationListException(params MessageException[] messages) : this(messages.ToList()) { }

        public ValidationListException(IEnumerable<MessageException> messages) : this() {
            Messages = [];
            Messages.AddRange(messages);
        }

        public ValidationListException(string message) : base(message) {
        }

        protected ValidationListException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected ValidationListException(string message, string property) : base(message, property) {
        }

        protected ValidationListException(string message, System.Exception innerException) : base(message, innerException) {
        }
    }
}
