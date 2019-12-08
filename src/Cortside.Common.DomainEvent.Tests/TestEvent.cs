namespace Cortside.Common.DomainEvent.Tests {
    public class TestEvent {
        public static TestEvent Instance { set; get; }
        public static string CorrelationId { set; get; }

        public int TheInt { set; get; }
        public string TheString { set; get; }
    }
}
