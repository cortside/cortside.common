using System.Collections.Generic;
using System.Linq;

namespace Cortside.Common.Messages.MessageExceptions {
    public class ValidationListException : MessageListException {
        public ValidationListException() : base("Validation failed") {
            Messages = new List<MessageException>();
        }

        public ValidationListException(params MessageException[] messages) : this(messages.ToList()) { }

        public ValidationListException(IEnumerable<MessageException> messages) : this() {
            Messages = new List<MessageException>();
            Messages.AddRange(messages);
        }
    }
}
