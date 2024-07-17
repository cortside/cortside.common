using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class UnprocessableEntityResponseException : MessageException {
        public UnprocessableEntityResponseException() : base() { }

        public UnprocessableEntityResponseException(string message) : base(message) { }

        public UnprocessableEntityResponseException(string message, Exception exception) : base(message, exception) { }

        protected UnprocessableEntityResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
