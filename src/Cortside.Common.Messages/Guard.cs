using System;

namespace Cortside.Common.Messages {
    public static class Guard {
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
