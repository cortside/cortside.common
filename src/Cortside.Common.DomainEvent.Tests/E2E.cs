using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2E : IDisposable {
        IConfigurationRoot configRoot;
        readonly IServiceProvider serviceProvider;
        Random r;

        public E2E() {
            r = new Random();

            //Config
            var config = new ConfigurationBuilder()
            .AddJsonFile("config.json");
            configRoot = config.Build();

            //IoC
            var collection = new ServiceCollection();
            collection.AddSingleton<IDomainEventHandler<TestEvent>, TestEventHandler>();
            serviceProvider = collection.BuildServiceProvider();
        }

        public void Dispose() {
            TestEvent.Instance = null;
        }

        [Trait("Category", "Integration")]
        [Fact(Skip = "Integraton test, needs running message broker")]
        public async Task ShouldBeAbleToSendAndReceive() {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var receiverLoggerMock = new Mock<ILogger<DomainEventReceiver>>();
            var receiverSection = configRoot.GetSection("Receiver.Settings");
            var receiverSettings = GetSettings<ServiceBusReceiverSettings>(receiverSection);
            var receiver = new DomainEventReceiver(receiverSettings, serviceProvider, receiverLoggerMock.Object);
            receiver.Closed += (r, e) => tokenSource.Cancel();

            var publisherLoggerMock = new Mock<ILogger<DomainEventPublisher>>();
            var publisherSection = configRoot.GetSection("Publisher.Settings");
            var publisherSettings = GetSettings<ServiceBusPublisherSettings>(publisherSection);
            var publisher = new DomainEventPublisher(publisherSettings, publisherLoggerMock.Object);

            var @event = new TestEvent {
                TheInt = r.Next(),
                TheString = Guid.NewGuid().ToString()
            };

            var correlationId = Guid.NewGuid().ToString();
            try {
                await publisher.SendAsync(@event, correlationId);
            } finally {
                Assert.Null(publisher.Error);
            }

            receiver.Receive(new Dictionary<string, Type> {
                { typeof(TestEvent).FullName,
                    typeof(TestEvent) }
            });
            var start = DateTime.Now;
            while (TestEvent.Instance == null && (DateTime.Now - start) < new TimeSpan(0, 0, 30)) {
                if (token.IsCancellationRequested) {
                    if (receiver.Error != null) {
                        Assert.Equal(string.Empty, receiver.Error.Description);
                        Assert.Equal(string.Empty, receiver.Error.Condition);
                    }
                    Assert.True(receiver.Error == null);
                }
                Thread.Sleep(1000);
            } // run for 30 seconds
            Assert.NotNull(TestEvent.Instance);
            Assert.NotNull(TestEvent.CorrelationId);
            Assert.Equal(correlationId, TestEvent.CorrelationId);
            Assert.Equal(@event.TheString, TestEvent.Instance.TheString);
            Assert.Equal(@event.TheInt, TestEvent.Instance.TheInt);
        }

        private T GetSettings<T>(IConfigurationSection section) where T : ServiceBusSettings, new() {
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
    }
}
