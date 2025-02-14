using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cortside.Common.Hosting.Tests {
    public partial class TimedHostedServiceTests {
        private readonly Mock<ILogger> logger;

        public TimedHostedServiceTests() {
            logger = new Mock<ILogger>();
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotConstructWithNullLogger() {
            Assert.Throws<ArgumentNullException>(() => new TestTimedHostedService(default, true, 5));
        }

        [Fact]
        public async Task CanCallExecuteAsync() {
            // Arrange
            var stoppingToken = CancellationToken.None;
            var instance = new TestTimedHostedService(logger.Object, true, 1);

            // Act
            instance.PublicExecuteAsync(stoppingToken);
            await Task.Delay(100);

            // Assert
            Assert.True(instance.Executed);
        }

        [Fact]
        public async Task ShouldNotTimeout() {
            // Arrange
            var stoppingToken = CancellationToken.None;
            var interval = 1;
            var service = new TestTimedHostedService(logger.Object, true, interval, 250);

            // Act
            service.PublicExecuteAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(interval));

            // Assert
            Assert.True(service.Executed);
        }

        [Fact]
        public async Task ShouldTimeout() {
            // Arrange
            var stoppingToken = CancellationToken.None;
            var interval = 1;
            var delay = Convert.ToInt32(TimeSpan.FromSeconds(interval).TotalMilliseconds + 250);
            var service = new TestTimedHostedService(logger.Object, true, interval, delay);

            // Act
            service.PublicExecuteAsync(stoppingToken);
            await Task.Delay(delay * 2);

            // Assert
            Assert.False(service.Executed);
        }
    }
}
