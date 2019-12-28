using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Cortside.Common.Correlation {

    public static class CorrelationContext {
        private static readonly AsyncLocal<string> CorrelationId = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> RequestId = new AsyncLocal<string>();

        public static string SetFromHttpContext(HttpContext context) {
            string correlationId = null;

            // check for correlationId from request
            context.Request.Headers.TryGetValue("Request-Id", out var requestIds);
            var requestId = requestIds.FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(requestId)) {
                correlationId = requestId;
            }

            // generate one if not set
            if (correlationId == null) {
                correlationId = Guid.NewGuid().ToString();
            }

            // set values
            SetCorrelationId(correlationId);
            SetRequestId(context.TraceIdentifier);
            return correlationId;
        }

        public static void SetCorrelationId(string correlationId) {
            if (string.IsNullOrWhiteSpace(correlationId)) {
                throw new ArgumentException("Correlation id cannot be null or empty", nameof(correlationId));
            }

            if (!string.IsNullOrWhiteSpace(CorrelationId.Value)) {
                throw new InvalidOperationException("Correlation id is already set");
            }

            CorrelationId.Value = correlationId;
        }

        public static void SetRequestId(string requestId) {
            if (string.IsNullOrWhiteSpace(requestId)) {
                throw new ArgumentException("Request id cannot be null or empty", nameof(requestId));
            }

            if (!string.IsNullOrWhiteSpace(RequestId.Value)) {
                throw new InvalidOperationException("Request id is already set");
            }

            RequestId.Value = requestId;
        }

        public static string GetCorrelationId() {
            return CorrelationId.Value;
        }

        public static string GetRequestId() {
            return RequestId.Value;
        }
    }
}
