namespace Cortside.Common.Messages.MessageExceptions {
    public class InvalidTypeFormatError : MessageException {
        public InvalidTypeFormatError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value), property, null) {
            Property = property;
            Value = value;
        }

        public string Value { get; }
    }
}
