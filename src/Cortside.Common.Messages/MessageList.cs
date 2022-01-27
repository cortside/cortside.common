using System;
using System.Collections;

namespace Cortside.Common.Messages {
    public class MessageList : CollectionBase {
        // TODO: this class needs to be completed

        public MessageList() {
        }

        public Message this[int index] {
            get { return (Message)List[index]; }
            set { List[index] = value; }
        }

        public void Add(Message value) {
            List.Add(value);
        }

        public void AddRange(IList messages) {
            foreach (Message message in messages) {
                Add(message);
            }
        }
    }
}
