namespace Cortside.Common.Messages.MessageExceptions {
    public class InvalidValueError : MessageException {
        public InvalidValueError(string property, string value) : base(string.Format("{1} is not a valid value for {0}.", property, value), property, null) {
            Property = property;
            Value = value;
        }

        public InvalidValueError(string property, string value, string reason) : base(string.Format("{1} is not a valid value for {0}, because {2}.", property, value, reason), property, null) {
            Property = property;
            Value = value;
        }

        public string Value { get; }
    }
}
