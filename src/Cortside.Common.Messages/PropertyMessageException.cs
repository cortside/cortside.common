namespace Cortside.Common.Messages {
    public class PropertyMessageException : MessageException {
        public PropertyMessageException(string property) : base($"{property} is required.", property, $"{property} is required.") {
            Property = property;
        }
    }
}
