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
            var correlationId = CorrelationContext.SetFromHttpContext(context);

            var a = Activity.Current;
            if (a.ParentId == null) {
                a.SetParentId(correlationId);
            }

            context.Response.OnStarting(() => {
                context.Response.Headers.Add("X-Correlation-Id", new[] { correlationId });
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId)) {
                await _next.Invoke(context).ConfigureAwait(false);
            }
        }
    }
}
