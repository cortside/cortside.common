using System;
using Cortside.Common.Testing.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Cortside.Common.Testing.Tests.Logging.Xunit {
    public class XunitLoggerTest {
        private readonly ILoggerFactory loggerFactory;

        public XunitLoggerTest(ITestOutputHelper output) {
            // Create a logger factory with a debug provider
            loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Cortside.Common", LogLevel.Trace)
                    .AddXunit(output);
            });
        }

        [Fact]
        public void TestLogger() {
            // Create a logger with the category name of the current class
            var logger = loggerFactory.CreateLogger<XunitLoggerTest>();

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
        }
    }
}
