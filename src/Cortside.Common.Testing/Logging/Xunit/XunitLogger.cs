using System;
using Cortside.Common.Testing.Transactions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Cortside.Common.Testing.Logging.Xunit {
    /// <summary>
    /// A logger that writes messages to xUnit ITestOutputHelper.
    /// </summary>
    public sealed class XunitLogger : ILogger {
        private readonly string name;
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initializes a new instance of the <see cref="XunitLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The xUnit output helper instance</param>
        public XunitLogger(string name, ITestOutputHelper output) {
            this.name = name;
            this.output = output;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) where TState : notnull {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) {
            // Everything is enabled unless the debugger is not attached
            return logLevel != LogLevel.None;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (formatter == null) {
                throw new ArgumentNullException(nameof(formatter));
            }
            if (!IsEnabled(logLevel)) {
                return;
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message)) {
                return;
            }

            message = $"[{name}][{logLevel}] {message}";

            if (exception != null) {
                message += Environment.NewLine + Environment.NewLine + exception;
            }

            output.WriteLine(message);
        }
    }
}
