using System;
using System.Threading;

namespace Cortside.Common.Correlation {
    public static class CorrelationContext {
        private static readonly AsyncLocal<string> CorrelationId = new AsyncLocal<string>();
        private static readonly AsyncLocal<string> RequestId = new AsyncLocal<string>();

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
