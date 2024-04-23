using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.BootStrap {
    public static class ServiceCollectionExtensions {
        /// <summary>
        /// Add scoped classes with specified suffix from assembly that contains T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedInterfacesBySuffix<T>(this IServiceCollection services, string suffix) where T : class {
            typeof(T).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith(suffix, StringComparison.InvariantCulture))
                            && x.GetTypeInfo().IsClass
                            && !x.GetTypeInfo().IsAbstract
                            && x.GetInterfaces().Length > 0)
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            return services;
        }

        public static IServiceCollection AddSingletonClassesBySuffix<T>(this IServiceCollection services, string suffix) where T : class {
            typeof(T).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith(suffix, StringComparison.InvariantCulture))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(x => services.AddSingleton(x));

            return services;
        }
    }
}
