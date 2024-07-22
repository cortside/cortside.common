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

        [Fact]
        public void ShouldGenerateErrorModel() {
            var filter = new MessageExceptionResponseFilter(new Logger<MessageExceptionResponseFilter>(new LoggerFactory()));

            var messages = new MessageList() {
                new MissingRequiredFieldError("property1"),
                new InvalidTypeFormatError("property2", "abc")
            };
            var ex = new ValidationListException(messages);
            var model = filter.GetErrorsModel(ex);

            Assert.NotNull(model);
            Assert.NotEmpty(model.Errors);
            Assert.Equal(2, model.Errors.Count);

            Assert.Equal("{\"Errors\":[{\"Type\":\"MissingRequiredFieldError\",\"Property\":\"property1\",\"Message\":\"property1 is required.\",\"Exception\":null},{\"Type\":\"InvalidTypeFormatError\",\"Property\":\"property2\",\"Message\":\"abc is not a valid value for property2.\",\"Exception\":null}]}", model.ToJson());
        }
    }
}
