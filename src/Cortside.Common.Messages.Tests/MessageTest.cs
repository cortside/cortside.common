using System;
using Cortside.Common.Messages.Formatters;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Tests.Exceptions;
using Xunit;

namespace Cortside.Common.Messages.Tests {
    public class MessageTest {
        [Fact]
        public void TestMissingFieldMessage() {
            TestMessage message = new TestMessage("Param1", "Param2");
            IMessageFormatter formatter = new SimpleFormatter();
            Assert.Equal("First parameter is Param1. Second parameter is Param2.", formatter.Format(message));
        }

        [Fact]
        public void TestMessageListException() {
            MessageList messages = new MessageList();
            for (int i = 0; i < 3; i++) {
                messages.Add(new TestMessage("Param1", "Param2"));
            }
            MessageListException ex = new MessageListException(messages);
            Assert.Equal(3, ex.Messages.Count);
            IMessageFormatter formatter = new SimpleFormatter();
            for (int i = 0; i < 3; i++) {
                Assert.Equal("First parameter is Param1. Second parameter is Param2.", formatter.Format(ex.Messages[i]));
            }
        }

        [Fact]
        public void TestMessageListExceptionString() {
            const string boringOldErrorMessage = "Error in the application.";
            MessageList messages = new MessageList();
            for (int i = 0; i < 3; i++) {
                messages.Add(new TestMessage("Param1", "Param2"));
            }
            MessageListException ex = new MessageListException(messages);
            string errorMessage = ex.Message;
            Assert.NotEqual(boringOldErrorMessage, errorMessage); // this is the undescriptive error message if we do not override it
        }

        [Theory]
        [InlineData(typeof(BadRequestResponseException))]
        [InlineData(typeof(ConflictResponseException))]
        [InlineData(typeof(ForbiddenAccessResponseException))]
        [InlineData(typeof(InternalServerErrorResponseException))]
        [InlineData(typeof(NotFoundResponseException))]
        [InlineData(typeof(PreconditionFailedResponseException))]
        [InlineData(typeof(UnauthorizedResponseException))]
        [InlineData(typeof(UnprocessableEntityResponseException))]
        public void TestMessageListExceptionWithResponseExceptions(Type exceptionType) {
            MessageList messages = new MessageList();
            for (int i = 0; i < 3; i++) {
                var ex = (MessageException)Activator.CreateInstance(exceptionType);
                Assert.NotNull(ex);
                messages.Add(ex);
            }
            Assert.Equal(3, messages.Count);

            var messageListException = new MessageListException(messages);
            Assert.Equal(3, messageListException.Messages.Count);
        }
    }
}
