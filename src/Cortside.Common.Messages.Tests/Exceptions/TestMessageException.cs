using System;

namespace Cortside.Common.Messages.Tests.Exceptions {
    /// <summary>
    /// Simple message for testing and to show how messages are created.
    /// </summary>
    [Serializable]
    public class TestMessageException : MessageException {
        public TestMessageException(string param1, string param2) : base("First parameter is {0}. Second parameter is {1}.", null, param1, param2) {
        }

        protected TestMessageException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected TestMessageException() : base() {
        }

        protected TestMessageException(string message) : base(message) {
        }

        protected TestMessageException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
