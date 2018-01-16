using System;
using System.Collections.Generic;
using Cortside.Common.Message;
using RabbitMQ.Client;

namespace Cortside.Common.RabbitMQ {

    public interface IRabbitMQHelper {

        String BindQueue(IModel channel, Type type, String queueName, Dictionary<String, Type> types);

        IConnection CreateConnection();

        void Publish(IMessage message);

        void Publish(IConnection connection, IMessage message);

        void Publish(IModel channel, IMessage message);

        void BindExchange(IModel channel);
    }
}
