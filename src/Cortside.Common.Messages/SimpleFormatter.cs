using System;

namespace Cortside.Common.Messages {
    public class SimpleFormatter : IMessageFormatter {
        public string Format(Message message) {
            return string.Format(message.Key, message.Properties);
        }
    }
}
