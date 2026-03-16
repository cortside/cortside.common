using System;
using Microsoft.Extensions.Logging;
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

        public ILogger CreateLogger(string categoryName) {
            return new XunitLogger(categoryName, output);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
        }
    }
}
