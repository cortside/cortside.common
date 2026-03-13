using System;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class MissingRequiredFieldError : MessageException {
        public MissingRequiredFieldError(string fieldName) : base(string.Format("{0} is required.", fieldName), fieldName) {
            FieldName = fieldName;
        }

        protected MissingRequiredFieldError(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected MissingRequiredFieldError() : base() {
        }

        protected MissingRequiredFieldError(string message, string property) : base(message, property) {
        }

        protected MissingRequiredFieldError(string message, Exception innerException) : base(message, innerException) {
        }

        public string FieldName { get; }
    }
}
