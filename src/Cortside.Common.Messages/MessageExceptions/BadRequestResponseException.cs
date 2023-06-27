using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class BadRequestResponseException : MessageException {
        public BadRequestResponseException() : base() { }

        public BadRequestResponseException(string message) : base(message) { }

        public BadRequestResponseException(string message, Exception exception) : base(message, exception) { }

        protected BadRequestResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected BadRequestResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected BadRequestResponseException(string message, string property) : base(message, property) {
        }
    }
}
