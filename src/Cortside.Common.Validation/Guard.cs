using System;

namespace Cortside.Common.Validation {
    public sealed class GuardClause {
    }

    public static class Guard {
        public static GuardClause From { get; } = new GuardClause();

        public static void Against<T>(Func<bool> check) where T : Exception, new() {
            if (check()) {
                throw new T();
            }
        }

        public static void Against(Func<bool> check, Func<Exception> exceptionFunc) {
            if (check()) {
                throw exceptionFunc();
            }
        }

        public static void Against(Func<bool> check, Exception exception) {
            if (check()) {
                throw exception;
            }
        }
    }
}
