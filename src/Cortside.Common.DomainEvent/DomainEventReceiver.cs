using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

        public DomainEventReceiver(ServiceBusSettings settings, IServiceProvider provider, ILogger<DomainEventComms> logger)
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
            Logger.LogDebug("received message");
            try {
                // Get the body
                var rawBody = message.Body as string;
                var typeString = message.ApplicationProperties[MESSAGE_TYPE_KEY] as string;
                Logger.LogInformation($"Event type key: {typeString}");
                var dataType = EventTypeLookup[typeString];
                Logger.LogInformation($"Event type: {dataType}");
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(dataType);
                Logger.LogInformation($"Event type handler: {handlerType}");

                var data = JsonConvert.DeserializeObject(rawBody, dataType);
                Logger.LogDebug($"deserialized {rawBody}");
                var handler = Provider.GetService(handlerType);
                Logger.LogInformation($"Event type handler instance: {handler.GetType()}");

                if (handler != null) {
                    //TODO: Update the way "Handle" is retrieved in a type safe way.
                    var method = handlerType.GetTypeInfo().GetDeclaredMethod("Handle");
                    await (Task)method.Invoke(handler, new object[] { data });

                    receiver.Accept(message);
                } else {
                    var desc = $"Handler not found for {typeString}";
                    Logger.LogWarning(desc);
                    receiver.Reject(message);
                }
            } catch (Exception ex) {
                Logger.LogError(101, ex, ex.Message);
                receiver.Reject(message);
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
