using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Messages.Filters {
    public class MessageExceptionResponseFilter : IActionFilter {
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for MessageExceptionResponseFilter
        /// </summary>
        /// <param name="logger"></param>
        public MessageExceptionResponseFilter(ILogger<MessageExceptionResponseFilter> logger) {
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context) {
            MessageException exception = context.Exception as MessageException;
            if (exception == null) {
                return;
            }

            if (exception is NotFoundResponseException) {
                context.Result = new NotFoundObjectResult(GetErrorsModel(exception));
            } else if (exception is ValidationListException) {
                context.Result = new BadRequestObjectResult(GetErrorsModel(exception));
            } else if (exception is BadRequestResponseException) {
                context.Result = new BadRequestObjectResult(GetErrorsModel(exception));
            } else if (exception is InternalServerErrorResponseException) {
                var result = new ObjectResult(GetErrorsModel(exception)) {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                context.Result = result;
            } else if (exception is UnprocessableEntityResponseException) {
                var result = new ObjectResult(GetErrorsModel(exception)) {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.Result = result;
            } else if (exception is ConflictResponseException) {
                var result = new ObjectResult(GetErrorsModel(exception)) {
                    StatusCode = StatusCodes.Status409Conflict
                };
                context.Result = result;
            } else if (exception is ForbiddenAccessResponseException) {
                var result = new ObjectResult(GetErrorsModel(exception)) {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                context.Result = result;
            } else {
                return;
            }

            context.ExceptionHandled = true;
        }

        public ErrorsModel GetErrorsModel(MessageException exception) {
            var errorsModel = new ErrorsModel();

            if (exception is MessageListException parent) {
                foreach (var ex in parent.Messages) {
                    errorsModel.AddError(ex);
                }
            } else {
                errorsModel.AddError(exception);
            }

            return errorsModel;
        }

        /// <summary>
        /// On action executing
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context) {
            // nothing to do
        }
    }
}
