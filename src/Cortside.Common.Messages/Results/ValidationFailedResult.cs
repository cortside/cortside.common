using Cortside.Common.Messages.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cortside.Common.Messages.Results {
    public class ValidationFailedResult : ObjectResult {
        public ValidationFailedResult(ModelStateDictionary modelState) : base(new ErrorsModel(modelState)) {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
