using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages {
    public abstract class MessageException : Exception {
        protected MessageException(string key, string property, params object[] properties) : base(key) {
            Key = key;
            Property = property;
            Properties = properties;
        }

        protected MessageException() : base() { }
        protected MessageException(string message) : base(message) { }

        protected MessageException(string message, string property) : base(message) {
            Property = property;
        }

        protected MessageException(string message, Exception innerException) : base(message, innerException) { }
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string Key { get; } = string.Empty;

        public object[] Properties { get; } = new object[0];
        public string Property { get; protected set; }
    }
}
