using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class UnprocessableEntityResponseException : MessageException {
        public UnprocessableEntityResponseException() : base() { }

        public UnprocessableEntityResponseException(string message) : base(message) { }

        public UnprocessableEntityResponseException(string message, Exception exception) : base(message, exception) { }

        protected UnprocessableEntityResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected UnprocessableEntityResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected UnprocessableEntityResponseException(string message, string property) : base(message, property) {
        }
    }
}
