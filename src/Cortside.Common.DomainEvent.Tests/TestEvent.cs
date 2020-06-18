using System.Collections.Generic;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEvent {
        public static Dictionary<string, TestEvent> Instances { get; } = new Dictionary<string, TestEvent>();

        public int TheInt { set; get; }
        public string TheString { set; get; }
    }
}
