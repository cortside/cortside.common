using System;

namespace Cortside.Common.Validation {
    public static class GuardExtensions {
        public static void Null(this GuardClause g, object input, string argumentName, string? errorMessage = null) {
            if (input == null) {
                throw new ArgumentException(errorMessage, argumentName);
            }
        }

        public static void NullOrEmpty(this GuardClause g, string? input, string argumentName, string? errorMessage = null) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentException(errorMessage, argumentName);
            }
        }

        public static void NullOrWhitespace(this GuardClause g, string? input, string argumentName, string? errorMessage = null) {
            if (string.IsNullOrWhiteSpace(input)) {
                throw new ArgumentException(errorMessage, argumentName);
            }
        }
    }
}
