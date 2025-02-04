using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Testing.Logging.LogEvent {
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class LogEventLoggerFactoryExtensions {
        /// <summary>
        /// Adds a debug logger named 'Debug' to the factory.
        /// </summary>
        /// <param name="builder">The extension method argument.</param>
        public static ILoggingBuilder AddLogEvent(this ILoggingBuilder builder) {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(new LogEventLoggerProvider()));

            return builder;
        }
    }
}
