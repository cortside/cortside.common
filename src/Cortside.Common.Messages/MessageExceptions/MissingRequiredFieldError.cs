using System;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class MissingRequiredFieldError : MessageException {
        public MissingRequiredFieldError(string fieldName) : base(string.Format("{0} is required.", fieldName), fieldName) {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
