using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class BadRequestResponseException : MessageException {
        public BadRequestResponseException() : base() { }

        public BadRequestResponseException(string message) : base(message) { }

        public BadRequestResponseException(string message, Exception exception) : base(message, exception) { }

        protected BadRequestResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
