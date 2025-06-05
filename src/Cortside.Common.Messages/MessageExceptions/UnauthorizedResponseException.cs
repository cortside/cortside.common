using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    public class UnauthorizedResponseException : MessageException {
        public UnauthorizedResponseException() : base() { }

        public UnauthorizedResponseException(string message) : base(message) { }

        public UnauthorizedResponseException(string message, Exception exception) : base(message, exception) { }

        protected UnauthorizedResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected UnauthorizedResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected UnauthorizedResponseException(string message, string property) : base(message, property) {
        }
    }
}
