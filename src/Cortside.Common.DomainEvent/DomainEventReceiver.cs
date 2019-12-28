using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventReceiver : DomainEventComms, IDomainEventReceiver, IDisposable {
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
            var messageType = message.ApplicationProperties[MESSAGE_TYPE_KEY] as string;
            using (Logger.BeginScope(new Dictionary<string, object> {
                ["CorrelationId"] = message.Properties.CorrelationId,
                ["MessageId"] = message.Properties.MessageId,
                ["MessageType"] = messageType
            })) {
                Logger.LogInformation($"Received message {message.Properties.MessageId}");
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
                        throw new ArgumentException($"Message {message.Properties.MessageId} has body with an invalid type {message.Body.GetType().ToString()}");
                    }

                    Logger.LogTrace($"Received message {message.Properties.MessageId} with body: {rawBody}");
                    Logger.LogDebug($"Event type key: {messageType}");
                    var dataType = EventTypeLookup[messageType];
                    Logger.LogDebug($"Event type: {dataType}");
                    var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(dataType);
                    Logger.LogDebug($"Event type handler interface: {handlerType}");
                    var handler = Provider.GetService(handlerType);

                    if (handler != null) {
                        Logger.LogDebug($"Event type handler: {handler.GetType()}");

                        var data = JsonConvert.DeserializeObject(rawBody, dataType);
                        Logger.LogDebug($"Successfully deserialized body to {dataType}");

                        //TODO: Update the way "Handle" is retrieved in a type safe way.
                        var eventType = typeof(DomainEventMessage<>).MakeGenericType(dataType);

                        var method1 = handlerType.GetTypeInfo().GetMethod("Handle", new Type[] { eventType });
                        var method2 = handlerType.GetTypeInfo().GetMethod("Handle", new Type[] { dataType });
                        if (method1 != null) {
                            dynamic domainEvent = Activator.CreateInstance(eventType);
                            domainEvent.MessageId = message.Properties.MessageId;
                            domainEvent.CorrelationId = message.Properties.CorrelationId;
                            domainEvent.Data = (dynamic)data;
                            await (Task)method1.Invoke(handler, new object[] { domainEvent });
                        } else {
                            await (Task)method2.Invoke(handler, new object[] { data });
                        }

                        receiver.Accept(message);
                        Logger.LogInformation($"Message {message.Properties.MessageId} accepted");
                    } else {
                        receiver.Reject(message);
                        Logger.LogError($"Message {message.Properties.MessageId} rejected because handler was not found for type {messageType}");
                    }
                } catch (Exception ex) {
                    receiver.Reject(message);
                    Logger.LogError(ex, $"Message {message.Properties.MessageId} rejected because of unhandled exception {ex.Message}");
                }
            }
        }

        public void Close(TimeSpan? timeout = null) {
            timeout = timeout ?? TimeSpan.Zero;
            Link?.Session.Close(timeout.Value);
            Link?.Session.Connection.Close(timeout.Value);
            Link?.Close(timeout.Value);
            Link = null;
            Error = null;
            EventTypeLookup = null;
        }

        public void Dispose() {
            this.Close();
        }
    }
}
