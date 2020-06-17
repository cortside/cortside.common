using Microsoft.Extensions.Logging;

namespace Cortside.Common.TestingUtilities {
    public class LogEvent {
        public LogLevel LogLevel { get; internal set; }
        public string Message { get; internal set; }
    }
}
