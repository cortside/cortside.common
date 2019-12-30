using System.Collections.Generic;

namespace Cortside.Common.DomainEvent.Tests {
    public class TestEvent {
        private static readonly Dictionary<string, TestEvent> instances = new Dictionary<string, TestEvent>();
        public static Dictionary<string, TestEvent> Instances {
            get { return instances; }
        }

        public int TheInt { set; get; }
        public string TheString { set; get; }
    }
}
