using System;
using Microsoft.Extensions.Configuration;

namespace Cortside.Common.IoC {
    public static class DI {
        private static IServiceProvider privateContainer;
        private static readonly object lockObject = new object();
        private static IConfiguration configuration;

        public static void SetContainer(IServiceProvider container) {
            lock (lockObject) {
                privateContainer = container;
            }
        }

        public static IServiceProvider Container {
            get { return privateContainer; }
        }

        public static void SetConfiguration(IConfiguration configuration) {
            lock (lockObject) {
                DI.configuration = configuration;
            }
        }

        public static IConfiguration Configuration {
            get { return configuration; }
        }
    }
}
