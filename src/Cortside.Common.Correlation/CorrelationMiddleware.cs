using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Cortside.Common.Correlation {
    /// <summary>
    /// Middleware to fix 2.2.x bug with HttpContextAccessor.
    /// This class should be the first thing at the top of the Configure method to ensure that it's around everything
    /// </summary>
    public class CorrelationMiddleware {
        private readonly RequestDelegate next;
        private readonly IHttpContextAccessor httpAccessor;

        public CorrelationMiddleware(RequestDelegate next, IHttpContextAccessor httpAccessor) {
            this.next = next;
            this.httpAccessor = httpAccessor;
        }

        public async Task InvokeAsync(HttpContext context) {
            var correlationId = CorrelationContext.SetFromHttpContext(context);

            var a = Activity.Current;
            if (a != null && a.ParentId == null) {
                a.SetParentId(correlationId);
            }

            context.Response.OnStarting(() => {
                context.Response.Headers.Add("X-Correlation-Id", new[] { correlationId });
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId)) {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }
    }
}
