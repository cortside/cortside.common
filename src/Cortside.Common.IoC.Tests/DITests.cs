using System;
using Cortside.Common.IoC;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Cortside.Common.Ioc.Tests {

    public static class DITests {
        [Fact]
        public static void CanCallSetContainer() {
            // Arrange
            var instance = new Mock<IServiceProvider>(MockBehavior.Strict).Object;

            // Act
            DI.SetContainer(instance);

            // Assert
            Assert.NotNull(DI.Container);
        }

        [Fact(Skip = "probably a good check to add")]
        public static void CannotCallSetContainerWithNullInstance() {
            Assert.Throws<ArgumentNullException>(() => DI.SetContainer(default(IServiceProvider)));
        }

        [Fact]
        public static void CanCallSetConfiguration() {
            // Arrange
            var instance = new Mock<IConfiguration>(MockBehavior.Strict).Object;

            // Act
            DI.SetConfiguration(instance);

            // Assert
            Assert.NotNull(DI.Configuration);
        }

        [Fact(Skip = "probably a good check to add")]
        public static void CannotCallSetConfigurationWithNullInstance() {
            Assert.Throws<ArgumentNullException>(() => DI.SetConfiguration(default(IConfiguration)));
        }

        [Fact]
        public static void CanGetContainer() {
            // Arrange
            var instance = new Mock<IServiceProvider>(MockBehavior.Strict).Object;

            // Act
            DI.SetContainer(instance);

            // Assert
            Assert.Same(instance, DI.Container);
        }

        [Fact]
        public static void CanGetConfiguration() {
            // Arrange
            var instance = new Mock<IConfiguration>(MockBehavior.Strict).Object;

            // Act
            DI.SetConfiguration(instance);

            // Assert
            Assert.Same(instance, DI.Configuration);
        }
    }
}
