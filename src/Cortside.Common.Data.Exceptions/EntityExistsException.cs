using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Data.Exceptions {
    [Serializable]
    public class EntityExistsException : Exception {
        public EntityExistsException() {
        }

        public EntityExistsException(string message) : base(message) {
        }

        public EntityExistsException(string message, Exception inner) : base(message, inner) {
        }

        protected EntityExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
