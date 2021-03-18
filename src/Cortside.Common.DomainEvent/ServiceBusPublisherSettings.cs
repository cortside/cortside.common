using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class ServiceBusPublisherSettings : ServiceBusSettings {
        public JsonSerializerSettings SerializerSettings { get; set; }
    }
}
