namespace Cortside.Common.DomainEvent {
    public abstract class ServiceBusSettings {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string AppName { set; get; }
        /// <summary>
        /// Gets or sets the address. Topic (azure SB) or exchange/queue (RabbitMQ)
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        /// <remarks>
        /// When used in the Publisher, this is used as a prefix
        /// where the type name of the event class is appended to the Address.
        /// Topics and exchange keys should be named appropriately.
        /// When used in the Receiver
        /// Azure SB {topic}/Subscriptions/{subscription}
        /// RabbitMQ {queue}
        /// </remarks>
        public string Address { set; get; }
        /// <summary>
        /// Gets or sets the protocol, amqp or ampqs.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public string Protocol { set; get; }
        /// <summary>
        /// Gets or sets the name of the shared access policy (Azure SB) or username (RabbitMQ).
        /// </summary>
        /// <value>
        /// The name of the policy.
        /// </value>
        public string PolicyName { set; get; }
        /// <summary>
        /// Gets or sets the key (Azure SB) or password (RabbitMQ).
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        /// <remarks>
        /// The key for Azure SB is from the shared access policy.
        /// Amqpnetlite will not accept a '/' in the key
        /// </remarks>
        public string Key { set; get; }
        /// <summary>
        /// Gets or sets the namespace url (Azure SB) or host (RabbitMQ)
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { set; get; }
        /// <summary>
        /// Gets or sets the credits. 
        /// </summary>
        /// <value>
        /// The credits.
        /// </value>
        /// <remarks>
        /// This is only used in the Receiver, to limit the number of simultaneous retrievals of messages.
        /// </remarks>
        public int Credits { set; get; } = 10;
        /// <summary>
        /// Set durability of queues and messages
        /// </summary>
        /// <value>
        /// 0 = Transient, 1 = Durable
        /// </value>
        /// <remarks>
        /// default value is 0.  Rabbit MQ will reject connection if these
        /// settings do not match.  Azure SB does not seem to care what this
        /// setting is.
        /// </remarks>
        public uint Durable { set; get; } = 0;
    }
}
