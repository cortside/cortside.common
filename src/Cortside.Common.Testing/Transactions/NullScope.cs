using System;

namespace Cortside.Common.Testing.Transactions {
    public class NullScope : IDisposable {
        public static NullScope Instance { get; } = new NullScope();
        bool disposed = false;

        private NullScope() {
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed && disposing) // only dispose once!
            {
                // perform cleanup for this object
            }
            disposed = true;
        }

        public void Dispose() {
            Dispose(true);

            // tell the GC not to finalize
            GC.SuppressFinalize(this);
        }

        ~NullScope() {
            Dispose(false);
        }
    }
}
