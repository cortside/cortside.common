using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages {
    public abstract class MessageException : Exception {
        private readonly object[] properties = new object[0];
        private readonly string key = string.Empty;

        protected MessageException(string key, params object[] properties) : base(key) {
            this.key = key;
            this.properties = properties;
        }


        protected MessageException() : base() { }
        protected MessageException(string message) : base(message) { }
        protected MessageException(string message, Exception innerException) : base(message, innerException) { }
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }


        public string Key {
            get {
                return key;
            }
        }

        public object[] Properties {
            get {
                return properties;
            }
        }

    }
}
