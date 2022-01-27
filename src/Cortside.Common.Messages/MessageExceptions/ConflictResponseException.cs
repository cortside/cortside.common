using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class ConflictResponseException : MessageException {
        public ConflictResponseException() : base() { }

        public ConflictResponseException(string message) : base(message) { }

        public ConflictResponseException(string message, Exception exception) : base(message, exception) { }

        protected ConflictResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
