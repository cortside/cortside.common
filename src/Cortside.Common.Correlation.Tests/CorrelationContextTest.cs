using Xunit;

namespace Cortside.Common.Correlation.Tests {
    public class CorrelationContextTest {
        [Fact]
        public void ShouldGetNullCorrelationId() {
            Assert.Null(CorrelationContext.GetCorrelationId(false));
        }

        [Fact]
        public void ShouldGetNewCorrelationId() {
            Assert.NotNull(CorrelationContext.GetCorrelationId());
        }
    }
}
