using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class PreconditionFailedResponseException : MessageException {
        public PreconditionFailedResponseException() { }

        public PreconditionFailedResponseException(string message) : base(message) { }

        public PreconditionFailedResponseException(string message, Exception exception) : base(message, exception) { }

        protected PreconditionFailedResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
