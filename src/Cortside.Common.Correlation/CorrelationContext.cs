using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Cortside.Common.Correlation {
    public static class CorrelationContext {
        private static readonly AsyncLocal<string> CorrelationId = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> RequestId = new AsyncLocal<string>();

        public static string SetFromHttpContext(HttpContext context) {
            // get possible request headers for correlationId           
            var xCorrelationId = GetHeaderValue(context, "X-Correlation-Id");
            var requestId = GetHeaderValue(context, "Request-Id");

            // set with most preferred header
            string correlationId = xCorrelationId;

            // set with requestId if it's not set already
            correlationId ??= requestId;

            // set with new guid if not set
            correlationId ??= Guid.NewGuid().ToString();

            // set values
            SetCorrelationId(correlationId);
            SetRequestId(context.TraceIdentifier);
            return correlationId;
        }

        private static string GetHeaderValue(HttpContext context, string header) {
            // check for correlationId as Request-Id from request
            context.Request.Headers.TryGetValue(header, out var ids);
            var id = ids.FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(id)) {
                return id;
            }

            return null;
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

        public static string GetCorrelationId(bool generateCorrelationId = true) {
            var correlationId = CorrelationId.Value;
            if (generateCorrelationId && string.IsNullOrWhiteSpace(correlationId)) {
                correlationId = Guid.NewGuid().ToString();
                SetCorrelationId(correlationId);
            }

            return CorrelationId.Value;
        }

        public static string GetRequestId() {
            return RequestId.Value;
        }
    }
}
