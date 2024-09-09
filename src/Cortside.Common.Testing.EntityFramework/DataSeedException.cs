using System;

namespace Acme.ShoppingCart.Exceptions {
    public class DataSeedException : Exception {
        public DataSeedException(string message) : base(message) {
        }

        public DataSeedException(string message, System.Exception exception) : base(message, exception) {
        }

        public DataSeedException() : base("Error Seeding DbSet") {
        }
    }
}
