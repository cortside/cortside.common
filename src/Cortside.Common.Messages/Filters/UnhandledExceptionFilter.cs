using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cortside.Common.Messages.Filters {
    /// <summary>
    /// Unhandled exception filter, should be last in chain
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter" />
    public class UnhandledExceptionFilter : IExceptionFilter {
        public void OnException(ExceptionContext context) {
            context.Result = new ObjectResult(new ErrorsModel(new InternalServerErrorResponseException("Unhandled exception", context.Exception))) {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
