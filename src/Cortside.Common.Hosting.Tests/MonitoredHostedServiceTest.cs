using System.Linq;
using Cortside.Common.Hosting.Tests.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Cortside.Common.Hosting.Tests {
    public class MonitoredHostedServiceTest {
        [Fact]
        public void ShouldResolveAllMonitoredServices() {
            var services = new ServiceCollection();
            var logger = new Mock<ILogger>();
            services.AddSingleton(logger.Object);
            services.AddHostedService<TestTimedHostedService>();
            services.AddSingleton(new TestTimedHostedConfiguration() { Enabled = true, Interval = 30 });
            var serviceProvider = services.BuildServiceProvider();

            var monitoredServices = serviceProvider.GetServices<IHostedService>()
                .Where(x =>
                    x.GetType().GetInterfaces().Any(y => y == typeof(IMonitoredHostedService))
                )
                .ToList();
            monitoredServices.ShouldNotBeNull();
            monitoredServices.ShouldNotBeEmpty();
            monitoredServices.ShouldContain(x => x.GetType() == typeof(TestTimedHostedService));

            var monitoredService = monitoredServices.First(x => x.GetType().GetInterfaces().Any(y => y == typeof(IMonitoredHostedService))) as IMonitoredHostedService;
            monitoredService.ShouldNotBeNull();
            monitoredService.Interval.TotalSeconds.ShouldBe(30);
            monitoredService.LastActivity.ShouldBeInRange(System.DateTime.UtcNow.AddSeconds(-1), System.DateTime.UtcNow.AddSeconds(1));
        }
    }
}
