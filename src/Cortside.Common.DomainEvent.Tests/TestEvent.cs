using System.Collections.Generic;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEvent {
        public static Dictionary<string, TestEvent> Instances { get; } = new Dictionary<string, TestEvent>();

        public int IntValue { set; get; }
        public string StringValue { set; get; }
    }
}
