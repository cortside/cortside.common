using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventReceiver : DomainEventComms, IDomainEventReceiver {
        public event ReceiverClosedCallback Closed;
        public IServiceProvider Provider { get; }
        public IDictionary<string, Type> EventTypeLookup { get; private set; }
        public ReceiverLink Link { get; private set; }
        public DomainEventError Error { get; set; }

        public DomainEventReceiver(ServiceBusReceiverSettings settings, IServiceProvider provider, ILogger<DomainEventComms> logger)
            : base(settings, logger) {
            Provider = provider;
        }

        public void Receive(IDictionary<string, Type> eventTypeLookup) {
            if (Link != null) {
                throw new InvalidOperationException("Already receiving.");
            }

            EventTypeLookup = eventTypeLookup;
            Logger.LogInformation($"Registering {eventTypeLookup.Count} event types:");
            foreach (var pair in eventTypeLookup) {
                Logger.LogInformation($"{pair.Key} = {pair.Value}");
            }

            Error = null;
            var session = CreateSession();
            var attach = new Attach() {
                Source = new Source() { Address = Settings.Address, Durable = Settings.Durable },
                Target = new Target() { Address = null }
            };
            Link = new ReceiverLink(session, Settings.AppName, attach, null);
            Link.Closed += OnClosed;
            Link.SetCredit(Settings.Credits, true); //Not sure if this is sufficient to renew credits...
            Link.Start(Settings.Credits, OnMessageCallback);
        }

        private void OnClosed(IAmqpObject sender, Error error) {
            if (sender.Error != null) {
                Error = new DomainEventError();
                Error.Condition = sender.Error.Condition.ToString();
                Error.Description = sender.Error.Description;
            }
            Closed(this, Error);
        }

        protected virtual async void OnMessageCallback(IReceiverLink receiver, Message message) {
            Logger.LogDebug("Received message");
            try {
                string rawBody = null;
                // Get the body
                if (message.Body is string) {
                    rawBody = message.Body as string;
                } else if (message.Body is byte[]) {
                    using (var reader = XmlDictionaryReader.CreateBinaryReader(
                        new MemoryStream(message.Body as byte[]),
                        null,
                        XmlDictionaryReaderQuotas.Max)) {
                        var doc = new XmlDocument();
                        doc.Load(reader);
                        rawBody = doc.InnerText;
                    }
                } else {
                    throw new ArgumentException($"Message body has an invalid type {message.Body.GetType().ToString()}");
                }
                var typeString = message.ApplicationProperties[MESSAGE_TYPE_KEY] as string;
                Logger.LogInformation($"Event type key: {typeString}");
                var dataType = EventTypeLookup[typeString];
                Logger.LogInformation($"Event type: {dataType}");
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(dataType);
                Logger.LogInformation($"Event type handler interface: {handlerType}");
                var handler = Provider.GetService(handlerType);

                if (handler != null) {
                    Logger.LogInformation($"Event type handler: {handler.GetType()}");

                    var data = JsonConvert.DeserializeObject(rawBody, dataType);
                    Logger.LogDebug($"Successfully deserialized body to {dataType}");

                    //TODO: Update the way "Handle" is retrieved in a type safe way.
                    var method = handlerType.GetTypeInfo().GetDeclaredMethod("Handle");
                    await (Task)method.Invoke(handler, new object[] { data });

                    receiver.Accept(message);
                    Logger.LogDebug("Message accepted");
                } else {
                    var desc = $"Handler not found for {typeString}";
                    Logger.LogWarning(desc);
                    receiver.Reject(message);
                    Logger.LogError("Message rejected");
                }
            } catch (Exception ex) {
                Logger.LogError(101, ex, ex.Message);
                receiver.Reject(message);
                Logger.LogError("Message rejected");
            }
        }

        public void Close(TimeSpan? timeout = null) {
            timeout = timeout ?? TimeSpan.Zero;
            Link.Session.Close(timeout.Value);
            Link.Close(timeout.Value);
            Link = null;
            Error = null;
            EventTypeLookup = null;
        }
    }
}
