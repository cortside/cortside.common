using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.BootStrap {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection RegisterClassesWithSuffix<T>(this IServiceCollection services, string suffix) where T : class {
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
    }
}
