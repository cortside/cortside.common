using System;
using System.Collections;

namespace Cortside.Common.Messages {
    /// <summary>
    /// Abstract base class for messages.
    /// </summary>
    public abstract class Message : ApplicationException {
        private readonly object[] properties = new object[0];
        private readonly string key = string.Empty;

        protected Message(string key, params object[] properties) : base(key) {
            this.key = key;
            this.properties = properties;
        }

        protected Message(string key, Exception innerException, params object[] properties) : base(key, innerException) {
            this.key = key;
            this.properties = properties;
        }

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
