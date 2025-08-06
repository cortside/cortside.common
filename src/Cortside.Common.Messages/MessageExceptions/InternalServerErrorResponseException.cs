using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class InternalServerErrorResponseException : MessageException {
        public InternalServerErrorResponseException() : base() { }

        public InternalServerErrorResponseException(string message) : base(message) { }

        public InternalServerErrorResponseException(string message, Exception exception) : base(message, exception) { }

        protected InternalServerErrorResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
