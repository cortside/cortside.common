using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class InvalidValueError : MessageException {
        public InvalidValueError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value), property, null) {
            Property = property;
            Value = value;
        }

        public InvalidValueError(string property, string value, string message) : base(message, property, null) {
            Property = property;
            Value = value;
        }

        protected InvalidValueError(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected InvalidValueError() : base() {
        }

        protected InvalidValueError(string message) : base(message) {
        }

        protected InvalidValueError(string message, Exception innerException) : base(message, innerException) {
        }

        protected InvalidValueError(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string Value { get; }
    }
}
