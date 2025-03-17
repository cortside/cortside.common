using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.Common.Logging;
using Cortside.Common.Testing.Logging.LogEvent;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cortside.Common.Testing.Tests.Logging.Xunit {
    public class LogEventLoggerTest {

        [Fact]
        public void TestLoggerWithScope() {
            var logger = new LogEventLogger<LogEventLoggerTest>();
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
            Assert.Equal(8, logger.LogEvents.Count);

            using (logger.PushProperties(new Dictionary<string, object>() {
                ["UserId"] = "xxx",
                ["ExtraProperty"] = "yyy",
            })) {
                logger.LogDebug("logged messaged that should have 2 properties with it");
            }

            // 10, adding 1 for the actual log and 1 for the being scope
            Assert.Equal(10, logger.LogEvents.Count);
            Assert.Equal("UserId=xxxExtraProperty=yyy",
                logger.LogEvents.First(x => x.LogLevel == LogLevel.None).Message);
        }
    }
}
