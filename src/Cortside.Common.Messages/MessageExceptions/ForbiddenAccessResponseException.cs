using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    public class ForbiddenAccessResponseException : MessageException {
        public ForbiddenAccessResponseException() : base() { }

        public ForbiddenAccessResponseException(string message) : base(message) { }

        public ForbiddenAccessResponseException(string message, Exception exception) : base(message, exception) { }

        protected ForbiddenAccessResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
