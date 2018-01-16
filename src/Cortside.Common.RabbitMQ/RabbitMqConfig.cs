namespace Cortside.Common.RabbitMQ {

    public class RabbitMqConfig {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public int MaxRetryAttempts { get; set; }
        public int PrefetchLimit { get; set; }
    }
}
