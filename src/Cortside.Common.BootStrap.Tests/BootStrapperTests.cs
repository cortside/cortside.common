using System.Collections.Generic;

namespace Cortside.Common.BootStrap.Tests {
    using System;
    using Cortside.Common.BootStrap;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class BootStrapperTests {
        private readonly BootStrapper instance;

        public BootStrapperTests() {
            instance = new BootStrapper();
        }

        [Fact]
        public void CanConstruct() {
            // Act
            var instance = new BootStrapper();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void CanCallAddInstaller() {
            // Arrange
            var installer = new Mock<IInstaller>().Object;

            // Act
            instance.AddInstaller(installer);

            // Assert
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotCallAddInstallerWithNullInstaller() {
            Assert.Throws<ArgumentNullException>(() => instance.AddInstaller(default(IInstaller)));
        }

        [Fact]
        public void CanCallInitIoCContainerWithInstallers() {
            // Arrange
            var installers = new[] { new Mock<IInstaller>().Object, new Mock<IInstaller>().Object, new Mock<IInstaller>().Object };

            // Act
            var result = instance.InitIoCContainer(installers);

            // Assert
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotCallInitIoCContainerWithInstallersWithNullInstallers() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(default(IInstaller[])));
        }

        [Fact]
        public void CanCallInitIoCContainerWithApplicationInstaller() {
            // Arrange
            var applicationInstaller = new Mock<IInstaller>().Object;

            // Act
            var result = instance.InitIoCContainer(applicationInstaller);

            // Assert
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotCallInitIoCContainerWithApplicationInstallerWithNullApplicationInstaller() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(default(IInstaller)));
        }

        [Fact]
        public void CanCallInitIoCContainerWithNoParameters() {
            // Act
            var result = instance.InitIoCContainer();

            // Assert
        }

        [Fact]
        public void CanCallInitIoCContainerWithServices() {
            // Arrange
            var services = new Mock<IServiceCollection>().Object;

            // Act
            var result = instance.InitIoCContainer(services);

            // Assert
        }

        [Fact]
        public void CannotCallInitIoCContainerWithServicesWithNullServices() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(default(IServiceCollection)));
        }

        [Fact]
        public void CanCallInitIoCContainerWithConfigAndServices() {
            // Arrange

            var values = new Dictionary<string, string> {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"},
                {"Nested:Key2", "NestedValue2"}
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(values);
            var services = new Mock<IServiceCollection>().Object;

            // Act
            var result = instance.InitIoCContainer(config, services);

            // Assert
        }

        [Fact(Skip = "probably a good check to add")]
        public void CannotCallInitIoCContainerWithConfigAndServicesWithNullConfig() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(default(IConfigurationBuilder), new Mock<IServiceCollection>().Object));
        }

        [Fact]
        public void CannotCallInitIoCContainerWithConfigAndServicesWithNullServices() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(new Mock<IConfigurationBuilder>().Object, default(IServiceCollection)));
        }

        [Fact]
        public void CanCallInitIoCContainerWithConfigurationAndServices() {
            // Arrange
            var configuration = new Mock<IConfiguration>().Object;
            var services = new Mock<IServiceCollection>().Object;

            // Act
            var result = instance.InitIoCContainer(configuration, services);

            // Assert
        }

        [Fact]
        public void CannotCallInitIoCContainerWithConfigurationAndServicesWithNullConfiguration() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(default(IConfiguration), new Mock<IServiceCollection>().Object));
        }

        [Fact]
        public void CannotCallInitIoCContainerWithConfigurationAndServicesWithNullServices() {
            Assert.Throws<ArgumentNullException>(() => instance.InitIoCContainer(new Mock<IConfiguration>().Object, default(IServiceCollection)));
        }
    }
}
