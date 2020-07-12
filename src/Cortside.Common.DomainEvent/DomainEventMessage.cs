using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Amqp;
using Newtonsoft.Json;

namespace Cortside.Common.DomainEvent {

    public class DomainEventMessage {

        internal static DomainEventMessage CreateGenericInstance(Type dataType, Message message) {
            string body = GetBody(message);

            List<string> errors = new List<string>();
            var data = JsonConvert.DeserializeObject(body, dataType,
                new JsonSerializerSettings {
                    Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) => {
                        errors.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }
                });
            if (errors.Any()) {
                throw new JsonSerializationException($"Message {message.Properties.MessageId} rejected because of errors deserializing messsage body: {string.Join(", ", errors)}");
            }

            var eventType = typeof(DomainEventMessage<>).MakeGenericType(dataType);
            dynamic domainEvent = Activator.CreateInstance(eventType);
            ((DomainEventMessage)domainEvent).Data = (dynamic)data;
            domainEvent.Message = message;

            return domainEvent;
        }

        internal static string GetBody(Message message) {
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

        internal Message Message { get; set; }
        public string MessageId => Message.Properties.MessageId;
        public string CorrelationId => Message.Properties.CorrelationId;
        public string MessageTypeName => Message.ApplicationProperties[DomainEventComms.MESSAGE_TYPE_KEY] as string;
        public object Data { get; internal set; }
    }

    public class DomainEventMessage<T> : DomainEventMessage {
        public new T Data => (T)base.Data;
    }
}
