using System;

namespace Cortside.Common.Testing.EntityFramework {
    public class DataSeedException : Exception {
        public DataSeedException(string message) : base(message) {
        }

        public DataSeedException(string message, Exception exception) : base(message, exception) {
        }

        public DataSeedException() : base("Error Seeding DbSet") {
        }
    }
}
