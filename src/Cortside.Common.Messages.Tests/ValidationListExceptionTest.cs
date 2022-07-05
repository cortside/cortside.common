using Cortside.Common.Messages.Filters;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Tests.Exceptions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.Common.Messages.Tests {
    public class ValidationListExceptionTest {
        [Fact]
        public void ValidationListExceptionString() {
            const string boringOldErrorMessage = "Error in the application.";
            MessageList messages = new MessageList();
            for (var i = 0; i < 3; i++) {
                messages.Add(new TestMessage("Param1", "Param2"));
            }
            MessageListException ex = new MessageListException(messages);
            string errorMessage = ex.Message;
            Assert.NotEqual(errorMessage, boringOldErrorMessage);
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
            Assert.Single(model.Errors);
            Assert.Equal(2, model.Errors[0].Fields.Count);


        }
    }
}
