using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2ETransactionTest : E2EBase {

        [Fact]
        public async Task ShouldBeAbleToSendAndReceive() {
            if (enabled) {
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

                EventMessage message;
                using (var receiver = new DomainEventReceiver(receiverSettings, serviceProvider, mockLogger)) {
                    receiver.Start(eventTypes);
                    message = receiver.Receive(TimeSpan.FromSeconds(1));
                    if (message != null) {
                        message.Accept();
                    }

                }
                Assert.DoesNotContain(mockLogger.LogEvents, x => x.LogLevel == LogLevel.Error);
                Assert.NotNull(message);

                Assert.NotNull(message);
                Assert.Equal(correlationId, message.CorrelationId);
                Assert.NotNull(message.MessageId);
                Assert.NotNull(message.MessageTypeName);
                //Assert.Equal(@event.TheString, message.Data.TheString);
                //Assert.Equal(@event.TheInt, message.Data.TheInt);
            }
        }
    }
}
