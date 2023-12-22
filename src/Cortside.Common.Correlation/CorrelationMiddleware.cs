using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Correlation {
    /// <summary>
    /// Middleware handling getting and setting correlationId/requestId headers.
    /// This class should be the first thing at the top of the Configure method to ensure that it's around everything.
    /// </summary>
    public class CorrelationMiddleware {
        private readonly RequestDelegate next;
        private readonly ILogger<CorrelationMiddleware> logger;

        public CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger) {
            this.next = next;
            this.logger = logger;
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

            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId })) {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }
    }
}
