using System;
using System.Collections.Generic;

namespace Cortside.Common.Messages {
    public class MessageList : List<MessageException> {
        public MessageList() : base() { }
        public MessageList(IEnumerable<MessageException> messages) : base(messages) { }

        public MessageList Aggregate<T>(Func<bool> check) where T : MessageException, new() {
            if (check()) {
                Add(new T());
            }

            return this;
        }

        public MessageList Aggregate(Func<bool> check, Func<MessageException> message) {
            if (check()) {
                Add(message());
            }

            return this;
        }

        public MessageList Aggregate(Func<bool> check, MessageException message) {
            if (check()) {
                Add(message);
            }

            return this;
        }

        public void ThrowIfAny() {
            if (Count > 0) {
                throw new MessageListException(this);
            }
        }

        public void ThrowIfAny<T>() where T : MessageListException, new() {
            if (Count > 0) {
                var ex = new T();
                ex.Messages.AddRange(this);
                throw ex;
            }
        }
    }
}
