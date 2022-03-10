namespace Cortside.Common.Messages.MessageExceptions {
    public class InvalidTypeFormatError : MessageException {
        public InvalidTypeFormatError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value)) {
            Property = property;
            Value = value;
        }

        public string Property { get; }

        public string Value { get; }
    }
}
