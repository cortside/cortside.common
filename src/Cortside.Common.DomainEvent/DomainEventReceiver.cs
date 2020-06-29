using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Amqp;
using Amqp.Framing;
using Amqp.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {
    public class DomainEventReceiver : DomainEventComms, IDomainEventReceiver, IDisposable {
        public event ReceiverClosedCallback Closed;
        public IServiceProvider Provider { get; protected set; }
        public IDictionary<string, Type> EventTypeLookup { get; protected set; }
        public ReceiverLink Link { get; private set; }
        public DomainEventError Error { get; set; }

        public DomainEventReceiver(ServiceBusReceiverSettings settings, IServiceProvider provider, ILogger<DomainEventComms> logger)
            : base(settings, logger) {
            Provider = provider;
        }

        public void Start(IDictionary<string, Type> eventTypeLookup) {
            InternalStart(eventTypeLookup);
        }

        public void StartAndListen(IDictionary<string, Type> eventTypeLookup) {
            InternalStart(eventTypeLookup);

            Link.Start(Settings.Credits, (link, msg) => {
                // fire and forget
                _ = OnMessageCallback(link, msg);
            });
        }

        private void InternalStart(IDictionary<string, Type> eventTypeLookup) {
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
                Source = new Source() {
                    Address = Settings.Address,
                    Durable = Settings.Durable
                },
                Target = new Target() {
                    Address = null
                }
            };
            Link = new ReceiverLink(session, Settings.AppName, attach, null);
            Link.Closed += OnClosed;
        }

        protected void OnClosed(IAmqpObject sender, Error error) {
            if (sender.Error != null) {
                Error = new DomainEventError();
                Error.Condition = sender.Error.Condition.ToString();
                Error.Description = sender.Error.Description;
            }
            if (Closed != null) {
                Closed(this, Error);
            }
        }

        public EventMessage Receive() {
            return Receive(TimeSpan.FromSeconds(60));
        }

        public EventMessage Receive(TimeSpan timeout) {
            Message message = Link.Receive(timeout);
            if (message == null) {
                return null;
            }

            return new EventMessage(message, Link);
        }

        protected async Task OnMessageCallback(IReceiverLink receiver, Message message) {
            var messageTypeName = message.ApplicationProperties[MESSAGE_TYPE_KEY] as string;
            var properties = new Dictionary<string, object> {
                ["CorrelationId"] = message.Properties.CorrelationId,
                ["MessageId"] = message.Properties.MessageId,
                ["MessageType"] = messageTypeName
            };

            using (Logger.BeginScope(properties)) {
                Logger.LogInformation($"Received message {message.Properties.MessageId}");

                try {
                    string body = GetBody(message);
                    Logger.LogTrace($"Received message {message.Properties.MessageId} with body: {body}");

                    Logger.LogDebug($"Event type key: {messageTypeName}");
                    if (!EventTypeLookup.ContainsKey(messageTypeName)) {
                        Logger.LogError($"Message {message.Properties.MessageId} rejected because message type was not registered for type {messageTypeName}");
                        receiver.Reject(message);
                        return;
                    }

                    var dataType = EventTypeLookup[messageTypeName];
                    Logger.LogDebug($"Event type: {dataType}");
                    var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(dataType);
                    Logger.LogDebug($"Event type handler interface: {handlerType}");
                    var handler = Provider.GetService(handlerType);
                    if (handler == null) {
                        Logger.LogError($"Message {message.Properties.MessageId} rejected because handler was not found for type {messageTypeName}");
                        receiver.Reject(message);
                        return;
                    }
                    Logger.LogDebug($"Event type handler: {handler.GetType()}");

                    List<string> errors = new List<string>();
                    var data = JsonConvert.DeserializeObject(body, dataType,
                        new JsonSerializerSettings {
                            Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) => {
                                errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            }
                        });
                    if (errors.Any()) {
                        Logger.LogError($"Message {message.Properties.MessageId} rejected because of errors deserializing messsage body: {string.Join(", ", errors)}");
                        receiver.Reject(message);
                        return;
                    }
                    Logger.LogDebug($"Successfully deserialized body to {dataType}");

                    var eventType = typeof(DomainEventMessage<>).MakeGenericType(dataType);
                    dynamic domainEvent = Activator.CreateInstance(eventType);
                    domainEvent.MessageId = message.Properties.MessageId;
                    domainEvent.CorrelationId = message.Properties.CorrelationId;
                    domainEvent.Data = (dynamic)data;

                    HandlerResult result;
                    dynamic dhandler = handler;
                    try {
                        result = await dhandler.HandleAsync(domainEvent);
                    } catch (Exception ex) {
                        Logger.LogError(ex, $"Message {message.Properties.MessageId} caught unhandled exception {ex.Message}");
                        result = HandlerResult.Release;
                    }
                    Logger.LogInformation($"Handler executed for message {message.Properties.MessageId} and returned result of {result}");

                    switch (result) {
                        case HandlerResult.Success:
                            receiver.Accept(message);
                            Logger.LogInformation($"Message {message.Properties.MessageId} accepted");
                            break;
                        case HandlerResult.Retry:
                            var deliveryCount = message.Header.DeliveryCount;
                            var delay = 10 * deliveryCount;
                            var scheduleTime = DateTime.UtcNow.AddSeconds(delay);
                            var fields = new Fields() {
                                { new Symbol(SCHEDULED_ENQUEUE_TIME_UTC), scheduleTime },
                                { new Symbol("ScheduledEnqueueTimeUtc"), scheduleTime }
                            };

                            receiver.Modify(message, true, false, fields);
                            Logger.LogInformation($"Message {message.Properties.MessageId} requeued with delay of {delay} seconds for {scheduleTime}");
                            break;
                        case HandlerResult.Failed:
                            receiver.Reject(message);
                            break;
                        case HandlerResult.Release:
                        default:
                            receiver.Release(message);
                            break;
                    }
                } catch (Exception ex) {
                    Logger.LogError(ex, $"Message {message.Properties.MessageId} rejected because of unhandled exception {ex.Message}");
                    receiver.Reject(message);
                }
            }
        }

        private static string GetBody(Message message) {
            string body = null;
            // Get the body
            if (message.Body is string) {
                body = message.Body as string;
            } else if (message.Body is byte[]) {
                using (var reader = XmlDictionaryReader.CreateBinaryReader(
                    new MemoryStream(message.Body as byte[]),
                    null,
                    XmlDictionaryReaderQuotas.Max)) {
                    var doc = new XmlDocument();
                    doc.Load(reader);
                    body = doc.InnerText;
                }
            } else {
                throw new ArgumentException($"Message {message.Properties.MessageId} has body with an invalid type {message.Body.GetType()}");
            }

            return body;
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
