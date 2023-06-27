using System;
using Microsoft.Extensions.Configuration;

namespace Cortside.Common.IoC {
    public static class DI {
        private static IServiceProvider serviceProvider;
        private static readonly object lockObject = new object();
        private static IConfiguration configuration;

        public static void SetContainer(IServiceProvider instance) {
            lock (lockObject) {
                serviceProvider = instance;
            }
        }

        public static IServiceProvider Container {
            get { return serviceProvider; }
        }

        public static void SetConfiguration(IConfiguration instance) {
            lock (lockObject) {
                DI.configuration = instance;
            }
        }

        public static IConfiguration Configuration {
            get { return configuration; }
        }
    }
}
