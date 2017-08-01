using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spring2.Common.DomainEvent {
    public class ServiceBusSettings {
	/// <summary>
	/// Gets or sets the name of the application.
	/// </summary>
	/// <value>
	/// The name of the application.
	/// </value>
	public string AppName { set; get; }
	/// <summary>
	/// Gets or sets the address.
	/// </summary>
	/// <value>
	/// The address.
	/// </value>
	/// <remarks>
	/// When used in the Publisher, this is used as a prefix
	/// where the type name of the event class is appended to the Address.
	/// Topics and exchange keys should be named appropriately.
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
	/// Gets or sets the name of the policy (Azure SB) or username (RabbitMQ).
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
	public string Key { set; get; }
	/// <summary>
	/// Gets or sets the namespace.
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
    }
}
