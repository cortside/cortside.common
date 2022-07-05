using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages {
    public abstract class MessageException : Exception {
        protected MessageException(string key, string field, string description, params object[] properties) : base(key) {
            Key = key;
            Field = field;
            Description = description;
            Properties = properties;
        }

        protected MessageException() : base() { }
        protected MessageException(string message) : base(message) { }

        protected MessageException(string message, string field, string description) : base(message) {
            Field = field;
            Description = description;
        }

        protected MessageException(string message, Exception innerException) : base(message, innerException) { }
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string Key { get; } = string.Empty;

        public object[] Properties { get; } = new object[0];
        public string Field { get; }
        public string Description { get; }
    }
}
