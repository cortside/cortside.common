using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Cortside.Common.Correlation {

    /// <summary>
    /// Middleware to fix 2.2.x bug with HttpContextAccessor
    /// </summary>
    public class CorrelationMiddleware {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpAccessor;

        public CorrelationMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor) {
            _next = next;
            _httpAccessor = httpAccessor;
        }

        public async Task InvokeAsync(HttpContext context) {
            //if (context.Request.Headers.ContainsKey("X-Correlation-Id")) {
            //    context.TraceIdentifier = context.Request.Headers["X-Correlation-Id"];
            //    // WORKAROUND: On ASP.NET Core 2.2.1 we need to re-store in AsyncLocal the new TraceId, HttpContext Pair
            //    _httpAccessor.HttpContext = context;
            //}

            //// Call the next delegate/middleware in the pipeline
            //await _next(context);

            var correlationId = CorrelationContext.SetFromHttpContext(context);
            //TODO: need to check if it exists first
            //context.Request.Headers.Add("Request-Id", correlationId);

            var a = Activity.Current;
            if (a.ParentId == null) {
                a.SetParentId(correlationId);
            }

            using (LogContext.PushProperty("CorrelationId", correlationId)) {
                await _next.Invoke(context);
            }
        }
    }
}
