using Cortside.Common.Testing.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Cortside.Common.Testing.Logging.LogEvent {
    /// <summary>
    /// The provider for the <see cref="XunitLogger"/>.
    /// </summary>
    [ProviderAlias("LogEvent")]
    public class LogEventLoggerProvider : ILoggerProvider {
        private readonly ITestOutputHelper output;

        /// <inheritdoc />
        public ILogger CreateLogger(string name) {
            return new LogEventLogger(name);
        }

        /// <inheritdoc />
        public void Dispose() {
        }
    }
}
