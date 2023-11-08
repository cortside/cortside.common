using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cortside.Common.Hosting.Tests {
    public class TimedHostedServiceTests {
        private class TestTimedHostedService : TimedHostedService {
            public TestTimedHostedService(ILogger logger, bool enabled, int interval, bool generateCorrelationId = true) : base(logger, enabled, interval, generateCorrelationId) {
            }

            public Task PublicExecuteAsync(CancellationToken stoppingToken) {
                return base.ExecuteAsync(stoppingToken);
            }

            protected override Task ExecuteIntervalAsync() {
                Executed = true;
                return Task.CompletedTask;
            }

            public bool Executed { get; set; } = false;
        }

        private readonly TestTimedHostedService instance;
        private readonly Mock<ILogger> logger;

        public TimedHostedServiceTests() {
            logger = new Mock<ILogger>();
            instance = new TestTimedHostedService(logger.Object, true, 500, false);
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotConstructWithNullLogger() {
            Assert.Throws<ArgumentNullException>(() => new TestTimedHostedService(default, true, 5000, false));
        }

        [Fact]
        public async Task CanCallExecuteAsync() {
            // Arrange
            var stoppingToken = CancellationToken.None;

            // Act
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            instance.PublicExecuteAsync(stoppingToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            await Task.Delay(1000);

            // Assert
            Assert.True(instance.Executed);
        }
    }
}
