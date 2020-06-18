using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cortside.Common.TestingUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2E {
        private readonly IConfigurationRoot configRoot;
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, Type> eventTypes;
        private readonly Random r;
        private readonly DomainEventPublisher publisher;
        private readonly MockLogger<DomainEventComms> mockLogger;

        public E2E() {
            r = new Random();

            //Config
            var config = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .AddJsonFile("config.user.json", true);
            configRoot = config.Build();

            //IoC
            var collection = new ServiceCollection();
            collection.AddSingleton<IDomainEventHandler<TestEvent>, TestEventHandler>();
            serviceProvider = collection.BuildServiceProvider();

            eventTypes = new Dictionary<string, Type> {
                { typeof(TestEvent).FullName, typeof(TestEvent) }
            };

            mockLogger = new MockLogger<DomainEventComms>();

            var publisherSection = configRoot.GetSection("Publisher.Settings");
            var publisherSettings = GetSettings<ServiceBusPublisherSettings>(publisherSection);
            publisher = new DomainEventPublisher(publisherSettings, mockLogger);
        }

        [Fact]
        public async Task ShouldBeAbleToSendAndReceive() {
            if (configRoot.GetValue<bool>("EnableE2ETests")) {
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

                ReceiveAndWait(correlationId);

                Assert.DoesNotContain(mockLogger.LogEvents, x => x.LogLevel == LogLevel.Error);

                Assert.True(TestEvent.Instances.Any());
                Assert.True(TestEvent.Instances.ContainsKey(correlationId));
                Assert.NotNull(TestEvent.Instances[correlationId]);
                Assert.Equal(@event.TheString, TestEvent.Instances[correlationId].TheString);
                Assert.Equal(@event.TheInt, TestEvent.Instances[correlationId].TheInt);
            }
        }

        [Fact]
        public async Task ShouldBeAbleToScheduleAndReceive() {
            if (configRoot.GetValue<bool>("EnableE2ETests")) {
                var @event = new TestEvent {
                    TheInt = r.Next(),
                    TheString = Guid.NewGuid().ToString()
                };

                var correlationId = Guid.NewGuid().ToString();
                try {
                    await publisher.ScheduleMessageAsync(@event, correlationId, DateTime.UtcNow.AddSeconds(20));
                } finally {
                    Assert.Null(publisher.Error);
                }

                var elapsed = ReceiveAndWait(correlationId);

                Assert.DoesNotContain(mockLogger.LogEvents, x => x.LogLevel == LogLevel.Error);

                Assert.True(elapsed.TotalSeconds >= 20, $"{elapsed.TotalSeconds} >= 20");
                Assert.True(TestEvent.Instances.Any());
                Assert.True(TestEvent.Instances.ContainsKey(correlationId));
                Assert.NotNull(TestEvent.Instances[correlationId]);
                Assert.Equal(@event.TheString, TestEvent.Instances[correlationId].TheString);
                Assert.Equal(@event.TheInt, TestEvent.Instances[correlationId].TheInt);
            }
        }

        private TimeSpan ReceiveAndWait(string correlationId) {
            var tokenSource = new CancellationTokenSource();
            var receiverSection = configRoot.GetSection("Receiver.Settings");
            var receiverSettings = GetSettings<ServiceBusReceiverSettings>(receiverSection);
            var start = DateTime.Now;

            using (var receiver = new DomainEventReceiver(receiverSettings, serviceProvider, mockLogger)) {
                receiver.Closed += (r, e) => tokenSource.Cancel();
                receiver.StartAndListen(eventTypes);

                while (!TestEvent.Instances.ContainsKey(correlationId) && (DateTime.Now - start) < new TimeSpan(0, 0, 30)) {
                    if (tokenSource.Token.IsCancellationRequested) {
                        if (receiver.Error != null) {
                            Assert.Equal(string.Empty, receiver.Error.Description);
                            Assert.Equal(string.Empty, receiver.Error.Condition);
                        }
                        Assert.True(receiver.Error == null);
                    }
                    Thread.Sleep(1000);
                } // run for 30 seconds
            }

            return DateTime.Now.Subtract(start);
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
