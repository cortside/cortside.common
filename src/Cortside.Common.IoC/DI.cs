using System;
using Microsoft.Extensions.Configuration;

namespace Cortside.Common.IoC {
    public static class DI {
        private static readonly object lockObject = new object();

        public static void SetContainer(IServiceProvider instance) {
            lock (lockObject) {
                Container = instance;
            }
        }

        public static IServiceProvider Container { get; private set; }

        public static void SetConfiguration(IConfiguration instance) {
            lock (lockObject) {
                Configuration = instance;
            }
        }

        public static IConfiguration Configuration { get; private set; }
    }
}
