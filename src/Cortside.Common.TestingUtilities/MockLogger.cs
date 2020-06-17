using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.TestingUtilities {
    public class MockLogger<T> : ILogger<T> {
        public List<LogEvent> LogEvents { get; }

        public MockLogger() {
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
            return new MockDisposable();
        }
    }
}
