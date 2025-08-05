using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Tests.Exceptions;
using Xunit;

namespace Cortside.Common.Messages.Tests {
    public class ValidationListExceptionTest {
        [Fact]
        public void HasMessageOfType() {
            // arrange
            MessageList messages = [
                new TestMessageException("Param1", "Param2"),
                new NotFoundResponseException()
            ];

            // act
            ValidationListException ex = new ValidationListException(messages);

            // assert
            Assert.True(ex.HasMessageOfType<TestMessageException>());
            Assert.True(ex.HasMessageOfType<NotFoundResponseException>());
            Assert.False(ex.HasMessageOfType<InvalidTypeFormatError>());
        }

        [Fact]
        public void ValidationListExceptionString() {
            const string boringOldErrorMessage = "Error in the application.";
            MessageList messages = [];
            for (var i = 0; i < 3; i++) {
                messages.Add(new TestMessageException("Param1", "Param2"));
            }
            ValidationListException ex = new ValidationListException(messages);
            string errorMessage = ex.Message;
            Assert.NotEqual(boringOldErrorMessage, errorMessage);
        }

        [Fact]
        public void CreateWithMessageException() {
            MessageException ex = new TestMessageException("Param1", "Param2");
            MessageListException exception = new ValidationListException(ex);
            Assert.True(exception.HasMessageOfType<TestMessageException>());
        }
    }
}
