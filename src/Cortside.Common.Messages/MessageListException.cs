using System;
using System.Text;

namespace Cortside.Common.Messages {
    public class MessageListException : ApplicationException {
        readonly MessageList messages = new MessageList();

        public MessageListException(MessageList messages) {
            this.messages = messages == null ? new MessageList() : messages;
        }

        public MessageList Messages {
            get {
                return messages;
            }
        }

        // overrides the Message property on the base class Exception
        public override string Message {
            get {
                if (messages.Count > 0) {
                    IMessageFormatter formatter = null;
                    //if (ClassRegistry.CanResolve(typeof(IMessageFormatter))) {
                    //    formatter = ClassRegistry.Resolve<IMessageFormatter>();
                    //} else {
                    formatter = new SimpleFormatter();
                    //}
                    StringBuilder sb = new StringBuilder();
                    foreach (Message m in messages) {
                        sb.Append(formatter.Format(m) + Environment.NewLine);
                    }
                    return sb.ToString();
                } else {
                    return base.Message;
                }
            }
        }
    }
}
