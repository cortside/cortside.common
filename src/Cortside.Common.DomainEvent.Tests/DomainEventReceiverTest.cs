using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Amqp;
using Amqp.Framing;
using Cortside.Common.TestingUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class DomainEventReceiverTest {

        private readonly IServiceProvider serviceProvider;
        private readonly ServiceBusReceiverSettings settings;
        private readonly MockLogger<DomainEventComms> logger;
        private readonly TestReceiver receiver;
        private readonly Mock<IReceiverLink> receiverLink;

        public DomainEventReceiverTest() {
            var collection = new ServiceCollection();
            collection.AddSingleton<IDomainEventHandler<TestEvent>, TestEventHandler>();
            serviceProvider = collection.BuildServiceProvider();

            settings = new ServiceBusReceiverSettings();

            logger = new MockLogger<DomainEventComms>();
            receiver = new TestReceiver(settings, serviceProvider, logger);
            receiver.Setup(new Dictionary<string, Type> {
                { typeof(TestEvent).FullName,
                    typeof(TestEvent) }
            });

            receiverLink = new Mock<IReceiverLink>();
        }

        [Fact]
        public void ShouldHandleWelformedJson() {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;
            var body = JsonConvert.SerializeObject(@event);

            Message message = CreateMessage(eventType, body);

            receiverLink.Setup(x => x.Accept(message));

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.DoesNotContain(logger.LogEvents, x => x.LogLevel == LogLevel.Error);
        }

        [Theory]
        [InlineData("{")]
        [InlineData("{ \"contractorId\": \"6677\", \"contractorNumber\": \"1037\" \"sponsorNumber\": \"2910\" }")]
        public void ShouldHandleMalformedJson(string body) {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;

            Message message = CreateMessage(eventType, body);

            receiverLink.Setup(x => x.Reject(message, null));

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.Contains(logger.LogEvents, x => x.LogLevel == LogLevel.Error && x.Message.Contains("errors deserializing messsage body"));
        }

        [Fact]
        public void ShouldHandleInvalidType() {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;
            var body = 1;

            Message message = CreateMessage(eventType, body);

            receiverLink.Setup(x => x.Reject(message, null));

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.Contains(logger.LogEvents, x => x.LogLevel == LogLevel.Error && x.Message.Contains("invalid type"));
        }

        [Fact]
        public void ShouldHandleByteArray() {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;
            var body = JsonConvert.SerializeObject(@event);

            Message message = CreateMessage(eventType, GetByteArray(body));
            receiverLink.Setup(x => x.Accept(message));

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.DoesNotContain(logger.LogEvents, x => x.LogLevel == LogLevel.Error);
        }

        [Fact]
        public void ShouldHandleMessageTypeNotFound() {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;
            var body = JsonConvert.SerializeObject(@event);

            Message message = CreateMessage(eventType, body);
            receiverLink.Setup(x => x.Reject(message, null));
            receiver.Setup(new Dictionary<string, Type>());

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.Contains(logger.LogEvents, x => x.LogLevel == LogLevel.Error && x.Message.Contains("message type was not registered for type"));
        }

        [Fact]
        public void ShouldHandleHandlerNotFound() {
            // arrange
            var @event = new TestEvent();
            var eventType = @event.GetType().FullName;
            var body = JsonConvert.SerializeObject(@event);

            Message message = CreateMessage(eventType, body);
            receiverLink.Setup(x => x.Reject(message, null));

            var provider = new ServiceCollection().BuildServiceProvider();
            receiver.SetProvider(provider);

            // act
            receiver.MessageCallback(receiverLink.Object, message);

            // assert
            receiverLink.VerifyAll();
            Assert.Contains(logger.LogEvents, x => x.LogLevel == LogLevel.Error && x.Message.Contains("handler was not found for type"));
        }

        private static Message CreateMessage(string eventType, object body) {
            var message = new Message(body) {
                ApplicationProperties = new ApplicationProperties(),
                Properties = new Properties() {
                    CorrelationId = Guid.NewGuid().ToString(),
                    MessageId = Guid.NewGuid().ToString()
                }
            };
            message.ApplicationProperties[DomainEventComms.MESSAGE_TYPE_KEY] = eventType;
            return message;
        }

        public byte[] GetByteArray(string body) {
            MemoryStream stream = new MemoryStream();
            DataContractSerializer s = new DataContractSerializer(typeof(string));
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream);
            writer.WriteStartDocument();
            s.WriteStartObject(writer, body);
            s.WriteObjectContent(writer, body);
            s.WriteEndObject(writer);
            writer.Flush();
            stream.Position = 0;

            return stream.ToArray();
        }
    }
}
