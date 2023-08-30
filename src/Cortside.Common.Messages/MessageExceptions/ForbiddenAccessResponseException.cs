using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class ForbiddenAccessResponseException : MessageException {
        public ForbiddenAccessResponseException() : base() { }

        public ForbiddenAccessResponseException(string message) : base(message) { }

        public ForbiddenAccessResponseException(string message, Exception exception) : base(message, exception) { }

        protected ForbiddenAccessResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected ForbiddenAccessResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected ForbiddenAccessResponseException(string message, string property) : base(message, property) {
        }
    }
}
