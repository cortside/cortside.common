using System;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class InvalidTypeFormatError : MessageException {
        public InvalidTypeFormatError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value), property, null) {
            Property = property;
            Value = value;
        }

        protected InvalidTypeFormatError(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected InvalidTypeFormatError() : base() {
        }

        protected InvalidTypeFormatError(string message) : base(message) {
        }

        protected InvalidTypeFormatError(string message, System.Exception innerException) : base(message, innerException) {
        }

        public string Value { get; }
    }
}
