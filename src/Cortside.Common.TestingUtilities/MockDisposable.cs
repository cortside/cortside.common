using System;

namespace Cortside.Common.TestingUtilities {
    public class MockDisposable : IDisposable {
        bool _disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!_disposed && disposing) // only dispose once!
            {
                // perform cleanup for this object
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);

            // tell the GC not to finalize
            GC.SuppressFinalize(this);
        }

        ~MockDisposable() {
            Dispose(false);
        }
    }
}
