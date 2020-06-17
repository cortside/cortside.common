using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Cortside.Common.Correlation {

    /// <summary>
    /// Middleware to set CorrelationId and inject into LogContext
    /// </summary>
    public class CorrelationMiddleware {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpAccessor;

        public CorrelationMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor) {
            _next = next;
            _httpAccessor = httpAccessor;
        }

        public async Task InvokeAsync(HttpContext context) {
            var correlationId = CorrelationContext.SetFromHttpContext(context);

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
