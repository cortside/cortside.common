using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.Common.Logging;
using Cortside.Common.Testing.Logging.LogEvent;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.Common.Testing.Tests.Logging.Xunit {
    public class LogEventLoggerTest {
        private readonly ILoggerFactory loggerFactory;

        public LogEventLoggerTest(ITestOutputHelper output) {
            // Create a logger factory with a debug provider
            loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Cortside.Common", LogLevel.Trace)
                    .AddLogEvent();
            });
        }

        [Fact]
        public void TestLogger() {
            // Create a logger with the category name of the current class
            var logger = loggerFactory.CreateLogger<LogEventLoggerTest>();

            Assert.NotNull(logger);

            // Log some messages with different log levels and message templates
            logger.LogTrace("This is a trace message.");
            logger.LogDebug("This is a debug message.");
            logger.LogInformation("Hello {Name}!", "World");
            logger.LogWarning("This is a warning message.");
            logger.LogError("This is an error message.");
            logger.LogCritical("This is a critical message.");

            // Use structured logging to capture complex data
            var person = new Person { Name = "Alice", Age = 25 };
            logger.LogInformation("Created a new person: {@Person}", person);

            // Use exception logging to capture the details of an exception
            try {
                throw new Exception("Something went wrong.");
            } catch (Exception ex) {
                logger.LogError(ex, "An exception occurred.");
            }

            // Use the logger to capture a log event
            Assert.Equal(8, LogEventLogger.LogEvents.Count);

            using (logger.PushProperties(new Dictionary<string, object>() {
                ["UserId"] = "xxx",
                ["ExtraProperty"] = "yyy",
            })) {
                logger.LogDebug("logged messaged that should have 2 properties with it");
            }

            // 10, adding 1 for the actual log and 1 for the being scope
            Assert.Equal(10, LogEventLogger.LogEvents.Count);
            Assert.Equal("UserId=xxxExtraProperty=yyy", LogEventLogger.LogEvents.First(x => x.LogLevel == LogLevel.None).Message);
        }
    }
}
