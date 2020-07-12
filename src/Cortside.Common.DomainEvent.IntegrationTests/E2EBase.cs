using System;
using System.Collections.Generic;
using Cortside.Common.TestingUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2EBase {
        protected readonly IServiceProvider serviceProvider;
        protected readonly Dictionary<string, Type> eventTypes;
        protected readonly Random r;
        protected readonly DomainEventPublisher publisher;
        protected readonly MockLogger<DomainEventComms> mockLogger;
        protected readonly ServiceBusReceiverSettings receiverSettings;
        protected readonly ServiceBusPublisherSettings publisherSettings;
        protected readonly bool enabled;

        public E2EBase() {
            r = new Random();

            //Config
            var config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("config.user.json", true);
            var configRoot = config.Build();

            //IoC
            var collection = new ServiceCollection();
            collection.AddSingleton<IDomainEventHandler<TestEvent>, TestEventHandler>();
            serviceProvider = collection.BuildServiceProvider();

            eventTypes = new Dictionary<string, Type> {
                { typeof(TestEvent).FullName, typeof(TestEvent) }
            };

            mockLogger = new MockLogger<DomainEventComms>();

            var publisherSection = configRoot.GetSection("Publisher.Settings");
            publisherSettings = GetSettings<ServiceBusPublisherSettings>(publisherSection);
            publisher = new DomainEventPublisher(publisherSettings, mockLogger);

            var receiverSection = configRoot.GetSection("Receiver.Settings");
            receiverSettings = GetSettings<ServiceBusReceiverSettings>(receiverSection);

            enabled = configRoot.GetValue<bool>("EnableE2ETests");
        }

        protected T GetSettings<T>(IConfigurationSection section) where T : ServiceBusSettings, new() {
            return new T {
                AppName = section["AppName"],
                Address = section["Address"],
                Key = section["Key"],
                Namespace = section["Namespace"],
                PolicyName = section["Policy"],
                Protocol = section["Protocol"],
                Durable = Convert.ToUInt32(section["Durable"])
            };
        }

        protected TestEvent NewTestEvent() {
            var @event = new TestEvent {
                IntValue = r.Next(),
                StringValue = Guid.NewGuid().ToString()
            };
            return @event;
        }
    }
}
