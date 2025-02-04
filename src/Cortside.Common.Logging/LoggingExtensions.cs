using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Logging {
    public static class LoggerExtensions {
        public static IDisposable PushProperty(this ILogger logger, string name, object value) {
            return logger.BeginScope(new Dictionary<string, object> { { name, value } });
        }

        public static IDisposable PushProperties(this ILogger logger, Dictionary<string, object> properties) {
            return logger.BeginScope(properties);
        }
    }
}
