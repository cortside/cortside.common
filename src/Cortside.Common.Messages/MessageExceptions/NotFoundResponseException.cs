using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class NotFoundResponseException : MessageException {
        public NotFoundResponseException() : base() { }

        public NotFoundResponseException(string message) : base(message) { }

        public NotFoundResponseException(string message, Exception exception) : base(message, exception) { }

        protected NotFoundResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
