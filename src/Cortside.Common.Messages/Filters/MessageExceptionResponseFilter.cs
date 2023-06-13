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
            if (!(context.Exception is MessageException exception)) {
                return;
            }

            switch (exception) {
                case NotFoundResponseException _:
                    context.Result = new NotFoundObjectResult(GetErrorsModel(exception));
                    break;
                case ValidationListException _:
                case BadRequestResponseException _:
                    context.Result = new BadRequestObjectResult(GetErrorsModel(exception));
                    break;
                case InternalServerErrorResponseException _:
                    context.Result = new ObjectResult(GetErrorsModel(exception)) {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                    break;
                case UnprocessableEntityResponseException _:
                    context.Result = new ObjectResult(GetErrorsModel(exception)) {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                    break;
                case ConflictResponseException _:
                    context.Result = new ObjectResult(GetErrorsModel(exception)) {
                        StatusCode = StatusCodes.Status409Conflict
                    };
                    break;
                case ForbiddenAccessResponseException _:
                    context.Result = new ObjectResult(GetErrorsModel(exception)) {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    break;
                default:
                    return;
            }

            var result = context.Result as ObjectResult;
            logger.LogDebug("Handled exception of type {Type}, returning status code of {StatusCode}", exception.GetType(), result?.StatusCode);
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
