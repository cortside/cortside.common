using System;

namespace Cortside.Common.Messages.MessageExceptions {
    [Serializable]
    public class InvalidValueError : MessageException {
        public InvalidValueError(string property, string value) : base(string.Format("`{1}` is not a valid value for {0}.", property, value), property) {
            Property = property;
            Value = value;
        }

        public string Value { get; }
    }
}
