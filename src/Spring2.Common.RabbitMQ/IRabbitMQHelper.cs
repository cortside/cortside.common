using Spring2.Common.Message;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Spring2.Common.RabbitMQ {

    public interface IRabbitMQHelper {

	String BindQueue(IModel channel, Type type, String queueName, Dictionary<String, Type> types);

	IConnection CreateConnection();

	void Publish(IMessage message);

	void Publish(IConnection connection, IMessage message);

	void Publish(IModel channel, IMessage message);

	void BindExchange(IModel channel);
    }
}