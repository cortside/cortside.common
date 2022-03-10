using System;
using System.Reflection;

namespace Cortside.Common.Validation {
    public static class GuardExtensions {
        public static void Null(this GuardClause g, object input, string argumentName, string? errorMessage = null) {
            if (input == null) {
                throw new ArgumentException(errorMessage, argumentName);
            }
        }

        public static void Null<T>(this GuardClause g, object input, string errorMessage) where T : Exception, new() {
            if (input == null) {
                Type classType = typeof(T);
                ConstructorInfo classConstructor = classType.GetConstructor(new Type[] { typeof(string) });
                T ex = (T)classConstructor.Invoke(parameters: new object[] { errorMessage });
                throw ex;
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
