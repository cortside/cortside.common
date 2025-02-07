using System;
using System.Collections.Generic;
using Cortside.Common.Testing.Transactions;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Testing.Logging.LogEvent {
    public class LogEventLogger : ILogger {
        public List<LogEvent> LogEvents { get; }

        public LogEventLogger() {
            LogEvents = new List<LogEvent>();
        }
        public LogEventLogger(string name) {
            LogEvents = new List<LogEvent>();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            var log = new LogEvent() {
                LogLevel = logLevel,
                Message = state.ToString()
            };
            LogEvents.Add(log);
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) {
            var s = string.Empty;
            if (state is IEnumerable<KeyValuePair<string, object>>) {
                var context = state as IEnumerable<KeyValuePair<string, object>>;
                foreach (var kp in context) {
                    s += kp.Key + "=" + kp.Value.ToString();
                }

                LogEvents.Add(new LogEvent() { LogLevel = LogLevel.None, Message = s });
            }

            return NullScope.Instance;
        }
    }
}
