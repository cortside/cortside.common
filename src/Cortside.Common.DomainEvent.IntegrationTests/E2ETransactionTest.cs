using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2ETransactionTest : E2EBase {

        [Fact]
        public async Task ShouldBeAbleToSendAndReceive() {
            if (enabled) {
                var @event = NewTestEvent();
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
                Assert.Equal(correlationId, message.Message.CorrelationId);
                Assert.NotNull(message.Message.MessageId);
                Assert.NotNull(message.Message.MessageTypeName);

                Assert.Equal(@event.StringValue, ((TestEvent)message.Message.Data).StringValue);
                Assert.Equal(@event.IntValue, ((TestEvent)message.Message.Data).IntValue);
            }
        }

        [Fact]
        public async Task ShouldUseTransactionScope() {
            var s = Guid.NewGuid().ToString();
            if (enabled) {
                int nMsgs = 10;
                var ids = new List<int>();

                for (int i = 0; i < nMsgs; i++) {
                    var @event = new TestEvent() {
                        IntValue = i,
                        StringValue = s
                    };
                    ids.Add(i);
                    await publisher.SendAsync(@event).ConfigureAwait(false);
                }

                var receiver = new DomainEventReceiver(receiverSettings, serviceProvider, mockLogger);
                receiver.Start(eventTypes);

                var message1 = receiver.Receive();
                var message2 = receiver.Receive();

                // ack message1 and send a new message in a txn
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    message1.Accept();
                    ids.Remove(message1.GetData<TestEvent>().IntValue);

                    var @event = new TestEvent() { IntValue = nMsgs + 1, StringValue = s };
                    await publisher.SendAsync(@event).ConfigureAwait(false);
                    ids.Add(@event.IntValue);

                    ts.Complete();
                }

                // ack message2 and send a new message in a txn but abort the txn
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    message2.Accept();
                    await publisher.SendAsync(message2.GetData<TestEvent>()).ConfigureAwait(false);
                }

                // release the message, since it shouldn't have been accepted above
                message2.Release();

                // receive all messages. should see the effect of the first txn
                for (int i = 1; i <= nMsgs; i++) {
                    var message = receiver.Receive();
                    message.Accept();

                    Assert.Contains(message.GetData<TestEvent>().IntValue, ids);
                    ids.Remove(message.GetData<TestEvent>().IntValue);
                }

                // at this point, the queue should have zero messages.
                // If there are messages, it is a bug in the broker.
                Assert.Empty(ids);

                // shouldn't be any messages left
                var empty = receiver.Receive(TimeSpan.FromSeconds(2));
                Assert.Null(empty);

                receiver.Close();
            }
        }
    }
}
