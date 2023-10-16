using Microsoft.Extensions.Logging;

namespace Cortside.Common.Testing.Logging {
    public class LogEvent {
        public LogLevel LogLevel { get; internal set; }
        public string Message { get; internal set; }
    }
}
