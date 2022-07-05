namespace Cortside.Common.Messages.MessageExceptions {
    public class PropertyMessageException : MessageException {
        public PropertyMessageException(string property) : base($"{property} is required.", property, $"{property} is required.") {
            this.Property = property;
        }

        public string Property { get; }
    }
}
