using System;
using System.Threading.Tasks;
using Cortside.Common.Threading.Tasks;
using Xunit;

namespace Cortside.Common.Threading.Tests {
    public class TaskExtensionsTest {
        private Task DoStuffAsync() {
            return Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        private async Task<bool> DoBoolStuffAsync() {
            await Task.Delay(TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
            return true;
        }

        [Fact]
        public Task ShouldThrowTimeoutExceptionAsync() {
            return Assert.ThrowsAsync<TimeoutException>(() => DoStuffAsync().WithTimeoutAsync(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public Task ShouldThrowTimeoutException2Async() {
            return Assert.ThrowsAsync<TimeoutException>(() => DoBoolStuffAsync().WithTimeoutAsync(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public async Task ShouldNotThrowTimeoutExceptionAsync() {
            await DoStuffAsync().WithTimeoutAsync(TimeSpan.FromMilliseconds(300));
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldNotThrowTimeoutException2Async() {
            await DoBoolStuffAsync().WithTimeoutAsync(TimeSpan.FromMilliseconds(300));
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldThrowTimeoutException3Async() {
            await DoStuffAsync().WithUnwrappedTimeoutAsync(TimeSpan.FromMilliseconds(100));
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldNotThrowTimeoutException3Async() {
            await DoStuffAsync().WithUnwrappedTimeoutAsync(TimeSpan.FromMilliseconds(300));
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldUnwrapTimeoutAsync() {
            var b = await DoBoolStuffAsync().WithUnwrappedTimeoutAsync(TimeSpan.FromMilliseconds(100));
            Assert.False(b);
        }

        [Fact]
        public async Task ShouldUnwrapBoolStuffAsync() {
            var b = await DoBoolStuffAsync().WithUnwrappedTimeoutAsync(TimeSpan.FromMilliseconds(300));
            Assert.True(b);
        }
    }
}
