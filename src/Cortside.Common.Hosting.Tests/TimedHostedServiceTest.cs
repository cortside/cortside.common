using System;
using System.Threading;
using System.Threading.Tasks;
using Cortside.Common.Hosting.Tests.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cortside.Common.Hosting.Tests {
    public class TimedHostedServiceTest {
        private readonly TestTimedHostedService instance;
        private readonly Mock<ILogger> logger;

        public TimedHostedServiceTest() {
            logger = new Mock<ILogger>();
            instance = new TestTimedHostedService(logger.Object, new TestTimedHostedConfiguration() { Enabled = true, Interval = 500 });
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotConstructWithNullLogger() {
            Assert.Throws<ArgumentNullException>(() => new TestTimedHostedService(default, new TestTimedHostedConfiguration() { Enabled = true, Interval = 5000 }));
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
