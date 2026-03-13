using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class NotFoundResponseException : MessageException {
        public NotFoundResponseException() { }

        public NotFoundResponseException(string message) : base(message) { }

        public NotFoundResponseException(string message, Exception exception) : base(message, exception) { }

        protected NotFoundResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected NotFoundResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected NotFoundResponseException(string message, string property) : base(message, property) {
        }
    }
}
