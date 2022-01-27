namespace Cortside.Common.Messages.Formatters {
    public class SimpleFormatter : IMessageFormatter {
        public string Format(MessageException message) {
            return string.Format(message.Key, message.Properties);
        }
    }
}
