namespace Cortside.Common.Hosting.Tests {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cortside.Common.Hosting;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class TimedHostedServiceTests {
        private class TestTimedHostedService : TimedHostedService {
            public TestTimedHostedService(ILogger logger, bool enabled, int interval, bool generateCorrelationId = true) : base(logger, enabled, interval, generateCorrelationId) {
            }

            public Task PublicExecuteAsync(CancellationToken stoppingToken) {
                return base.ExecuteAsync(stoppingToken);
            }

            protected override Task ExecuteIntervalAsync() {
                Executed = true;
                return default(Task);
            }

            public bool Executed { get; set; } = false;
        }

        private TestTimedHostedService instance;
        private Mock<ILogger> logger;

        public TimedHostedServiceTests() {
            logger = new Mock<ILogger>();
            instance = new TestTimedHostedService(logger.Object, true, 500, false);
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotConstructWithNullLogger() {
            Assert.Throws<ArgumentNullException>(() => new TestTimedHostedService(default(ILogger), true, 5000, false));
        }

        [Fact]
        public async Task CanCallExecuteAsync() {
            // Arrange
            var stoppingToken = CancellationToken.None;

            // Act
            instance.PublicExecuteAsync(stoppingToken);
            await Task.Delay(1000).ConfigureAwait(false);

            // Assert
            Assert.True(instance.Executed);
        }
    }
}
