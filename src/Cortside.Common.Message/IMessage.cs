using System;

namespace Cortside.Common.Message {

    public interface IMessage {
        String MessageId { get; set; }
        String MessageType { get; set; }
        String RoutingKey { get; set; }
        String SourceMessageId { get; set; }
        DateTime Timestamp { get; set; }
    }
}
