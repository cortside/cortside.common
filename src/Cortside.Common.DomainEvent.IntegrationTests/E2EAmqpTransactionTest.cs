using System;
using System.Collections.Generic;
using System.Transactions;
using Amqp;
using Amqp.Framing;
using Xunit;

namespace Cortside.Common.DomainEvent.Tests {
    public class E2EAmqpTransactionTest : E2EBase {
        private readonly Amqp.Address address;
        private string path;

        public E2EAmqpTransactionTest() : base() {
            address = new Amqp.Address($"{base.publisherSettings.Protocol}://{base.publisherSettings.PolicyName}:{base.publisherSettings.Key}@{base.publisherSettings.Namespace}");
            path = base.publisherSettings.Address;
        }

        [Fact]
        public void TransactedPosting() {
            string testName = "TransactedPosting";
            path = "test." + testName;
            int nMsgs = 5;

            Connection connection = new Connection(address);
            Session session = new Session(connection);
            SenderLink sender = new SenderLink(session, "sender-" + testName, path);

            // commit
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = 0; i < nMsgs; i++) {
                    Message message = new Message("test");
                    message.Properties = new Properties() { MessageId = "commit" + i, GroupId = testName };
                    sender.Send(message);
                }

                ts.Complete();
            }

            // rollback
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = nMsgs; i < nMsgs * 2; i++) {
                    Message message = new Message("test");
                    message.Properties = new Properties() { MessageId = "rollback" + i, GroupId = testName };
                    sender.Send(message);
                }
            }

            // commit
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = 0; i < nMsgs; i++) {
                    Message message = new Message("test");
                    message.Properties = new Properties() { MessageId = "commit" + i, GroupId = testName };
                    sender.Send(message);
                }

                ts.Complete();
            }

            Connection connection2 = new Connection(address);
            Session session2 = new Session(connection2);
            ReceiverLink receiver = new ReceiverLink(session2, "receiver-" + testName, path);
            for (int i = 0; i < nMsgs * 2; i++) {
                Message message = receiver.Receive();
                Trace.WriteLine(TraceLevel.Information, "receive: {0}", message.Properties.MessageId);
                receiver.Accept(message);
                Assert.StartsWith("commit", message.Properties.MessageId);
            }

            // shouldn't be any messages left
            Message empty = receiver.Receive(TimeSpan.Zero);
            Assert.Null(empty);

            connection.Close();
            connection2.Close();
        }

        [Fact]
        public void TransactedRetiring() {
            string testName = "TransactedRetiring";
            path = "test." + testName;
            int nMsgs = 10;
            var ids = new List<string>();

            Connection connection = new Connection(address);
            Session session = new Session(connection);
            SenderLink sender = new SenderLink(session, "sender-" + testName, path);

            // send one extra for validation
            for (int i = 0; i < nMsgs + 1; i++) {
                Message message = new Message("test");
                message.Properties = new Properties() { MessageId = "msg" + i, GroupId = testName };
                ids.Add(message.Properties.MessageId);
                sender.Send(message);
            }

            ReceiverLink receiver = new ReceiverLink(session, "receiver-" + testName, path);
            Message[] messages = new Message[nMsgs];
            for (int i = 0; i < nMsgs; i++) {
                messages[i] = receiver.Receive();
                Trace.WriteLine(TraceLevel.Information, "receive: {0}", messages[i].Properties.MessageId);
            }

            // commit half
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = 0; i < nMsgs / 2; i++) {
                    receiver.Accept(messages[i]);
                    ids.Remove(messages[i].Properties.MessageId);
                }

                ts.Complete();
            }

            // rollback
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = nMsgs / 2; i < nMsgs; i++) {
                    receiver.Accept(messages[i]);
                }
            }

            // after rollback, messages should be still acquired
            {
                Message message = receiver.Receive();
                Assert.Contains(message.Properties.MessageId, ids);
                receiver.Release(message);
            }

            // commit
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                for (int i = nMsgs / 2; i < nMsgs; i++) {
                    receiver.Accept(messages[i]);
                    ids.Remove(messages[i].Properties.MessageId);
                }

                ts.Complete();
            }

            // only the last message is left
            {
                Message message = receiver.Receive();
                receiver.Accept(message);
                ids.Remove(message.Properties.MessageId);
            }

            // at this point, the queue should have zero messages.
            // If there are messages, it is a bug in the broker.
            Assert.Empty(ids);

            // shouldn't be any messages left
            Message empty = receiver.Receive(TimeSpan.Zero);
            Assert.Null(empty);

            connection.Close();
        }

        [Fact]
        public void TransactedRetiringAndPosting() {
            string testName = "TransactedRetiringAndPosting";
            path = "test." + testName;
            int nMsgs = 10;
            var ids = new List<string>();

            Connection connection = new Connection(address);
            Session session = new Session(connection);

            SenderLink sender = new SenderLink(session, "sender-" + testName, path);

            for (int i = 0; i < nMsgs; i++) {
                Message message = new Message("test") {
                    Properties = new Properties() {
                        MessageId = "msg" + i,
                        GroupId = testName
                    }
                };
                ids.Add(message.Properties.MessageId);
                sender.Send(message);
            }

            ReceiverLink receiver = new ReceiverLink(session, "receiver-" + testName, path);

            receiver.SetCredit(2, false);
            Message message1 = receiver.Receive();
            Message message2 = receiver.Receive();

            // ack message1 and send a new message in a txn
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                receiver.Accept(message1);
                ids.Remove(message1.Properties.MessageId);

                Message message = new Message("test");
                message.Properties = new Properties() { MessageId = "msg" + nMsgs, GroupId = testName };
                ids.Add(message.Properties.MessageId);
                sender.Send(message);

                ts.Complete();
            }

            // ack message2 and send a new message in a txn but abort the txn
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                receiver.Accept(message2);

                Message message = new Message("test");
                message.Properties = new Properties() { MessageId = "msg" + (nMsgs + 1), GroupId = testName };
                sender.Send(message1);
            }

            // release the message, since it shouldn't have been accepted above
            receiver.Release(message2);

            // receive all messages. should see the effect of the first txn
            receiver.SetCredit(nMsgs, false);
            for (int i = 1; i <= nMsgs; i++) {
                Message message = receiver.Receive();
                Trace.WriteLine(TraceLevel.Information, "receive: {0}", message.Properties.MessageId);
                receiver.Accept(message);

                Assert.Contains(message.Properties.MessageId, ids);
                ids.Remove(message.Properties.MessageId);
            }

            // at this point, the queue should have zero messages.
            // If there are messages, it is a bug in the broker.
            Assert.Empty(ids);

            // shouldn't be any messages left
            Message empty = receiver.Receive(TimeSpan.Zero);
            Assert.Null(empty);

            connection.Close();
        }
    }
}
