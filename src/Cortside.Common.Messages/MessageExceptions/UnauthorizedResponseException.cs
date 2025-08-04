using System;
using System.Runtime.Serialization;

namespace Cortside.Common.Messages.MessageExceptions {
    /// <summary>
    /// Exception thrown when a request is not authorized to access a resource.
    /// Use this exception to indicate that the user or client is authenticated but does not have permission to perform the requested action.
    /// This differs from other authorization-related exceptions such as <c>ForbiddenResponseException</c>, which may indicate different authorization failures.
    /// </summary>
    [Serializable]
    public class UnauthorizedResponseException : MessageException {
        public UnauthorizedResponseException() : base() { }

        public UnauthorizedResponseException(string message) : base(message) { }

        public UnauthorizedResponseException(string message, Exception exception) : base(message, exception) { }

        protected UnauthorizedResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected UnauthorizedResponseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected UnauthorizedResponseException(string message, string property) : base(message, property) {
        }
    }
}
