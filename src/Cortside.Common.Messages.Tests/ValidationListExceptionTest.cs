using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Tests.Exceptions;
using Xunit;

namespace Cortside.Common.Messages.Tests {
    public class ValidationListExceptionTest {
        [Fact]
        public void HasMessageOfType() {
            // arrange
            MessageList messages = new MessageList {
                new TestMessage("Param1", "Param2"),
                new NotFoundResponseException()
            };

            // act
            MessageListException ex = new MessageListException(messages);

            // assert
            Assert.True(ex.HasMessageOfType<TestMessage>());
            Assert.True(ex.HasMessageOfType<NotFoundResponseException>());
            Assert.False(ex.HasMessageOfType<InvalidTypeFormatError>());
        }

        [Fact]
        public void ValidationListExceptionString() {
            const string boringOldErrorMessage = "Error in the application.";
            MessageList messages = new MessageList();
            for (var i = 0; i < 3; i++) {
                messages.Add(new TestMessage("Param1", "Param2"));
            }
            MessageListException ex = new MessageListException(messages);
            string errorMessage = ex.Message;
            Assert.NotEqual(boringOldErrorMessage, errorMessage);
        }
    }
}
