using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Xunit.Abstractions;

namespace Cortside.Common.Testing.Logging.Xunit {
    /// <summary>
    /// The provider for the <see cref="XunitLogger"/>.
    /// </summary>
    [ProviderAlias("Xunit")]
    public class XunitLoggerProvider : ILoggerProvider {
        private readonly ITestOutputHelper output;

        public XunitLoggerProvider(ITestOutputHelper output) {
            this.output = output;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name) {
            return new XunitLogger(name, output);
        }

        /// <inheritdoc />
        public void Dispose() {
        }
    }
}
