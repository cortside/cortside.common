using Spring2.Common.Message;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Spring2.Common.RabbitMQ {

    public class RabbitMQHelper : IRabbitMQHelper {
	private ConnectionFactory factory;
	private String exchange;

	public RabbitMQHelper(IOptions<RabbitMqConfig> config) {
	    ConnectionFactory.DefaultSocketFactory(AddressFamily.InterNetwork);

	    var factory = new ConnectionFactory();
	    factory.HostName = config.Value.HostName;
	    factory.UserName = config.Value.UserName;
	    factory.Password = config.Value.Password;
	    factory.VirtualHost = config.Value.VirtualHost;

	    this.exchange = config.Value.Exchange;

	    Log.Information(factory.HostName + " " + factory.VirtualHost + " " + factory.UserName);
	    this.factory = factory;
	}

	public IConnection CreateConnection() {
	    return factory.CreateConnection();
	}

	public void Publish(IMessage message) {
	    String exchangeName = exchange;
	    using (var connection = factory.CreateConnection()) {
		using (var channel = connection.CreateModel()) {
		    channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true);

		    var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
		    String json = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSerializerSettings);

		    var body = Encoding.UTF8.GetBytes(json);

		    IBasicProperties props = channel.CreateBasicProperties();
		    props.CorrelationId = message.MessageId;
		    props.MessageId = message.MessageId;
		    props.DeliveryMode = 2;
		    props.Type = message.MessageType;

		    channel.BasicPublish(exchange: exchangeName, routingKey: message.RoutingKey, basicProperties: props, body: body, mandatory: true);
		    Log.Debug("Published message: {0}", json);
		}
	    }
	}

	public void Publish(IConnection connection, IMessage message) {
	    String exchangeName = exchange;
	    using (var channel = connection.CreateModel()) {
		channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true);

		var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
		String json = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSerializerSettings);

		var body = Encoding.UTF8.GetBytes(json);

		IBasicProperties props = channel.CreateBasicProperties();
		props.CorrelationId = message.MessageId;
		props.MessageId = message.MessageId;
		props.DeliveryMode = 2;
		props.Type = message.MessageType;

		channel.BasicPublish(exchange: exchangeName, routingKey: message.RoutingKey, basicProperties: props, body: body, mandatory: true);
		Log.Debug("Published message: {0}", json);
	    }
	}

	public void Publish(IModel channel, IMessage message) {
	    var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
	    String json = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSerializerSettings);

	    var body = Encoding.UTF8.GetBytes(json);

	    IBasicProperties props = channel.CreateBasicProperties();
	    props.CorrelationId = message.MessageId;
	    props.MessageId = message.MessageId;
	    props.DeliveryMode = 2;
	    props.Type = message.MessageType;

	    channel.BasicPublish(exchange: exchange, routingKey: message.RoutingKey, basicProperties: props, body: body, mandatory: true);
	    Log.Debug("Published message: {0}", json);
	}

	public String BindQueue(IModel channel, Type type, String queueName, Dictionary<String, Type> types) {
	    types.Add(type.FullName, type);

	    var routingKey = "#." + type.Name;

	    // setup the dlx
	    var dlxName = exchange + ".dead";
	    BindExchange(channel, dlxName);

	    // setup the dlq
	    var dlqName = queueName + ".dead";
	    channel.QueueDeclare(dlqName, true, false, false, null);
	    channel.QueueBind(queue: dlqName, exchange: dlxName, routingKey: routingKey);

	    BindExchange(channel);
	    IDictionary<string, object> arguments = new Dictionary<string, object> {
		{ "x-dead-letter-exchange", dlxName }
	    };
	    var queue = channel.QueueDeclare(queueName, true, false, false, arguments);
	    channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);
	    Log.Information("Registering type {0}, binding {1} with routing key {2} to queue from {3} exchange", type.AssemblyQualifiedName, queueName, routingKey, exchange);

	    return queueName;
	}

	public void BindExchange(IModel channel) {
	    BindExchange(channel, exchange);
	}

	public void BindExchange(IModel channel, String exchangeName) {
	    channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true);
	}
    }
}