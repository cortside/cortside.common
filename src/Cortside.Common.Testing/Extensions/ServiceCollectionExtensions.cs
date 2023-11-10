using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.Testing.Extensions {
    public static class ServiceCollectionExtensions {
        /// <summary>
        /// Removes a registered type
        /// </summary>
        /// <remarks>
        /// Will search for and find first instance and remove it if it is found. No error when not found.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Unregister<T>(this IServiceCollection services) where T : class {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            return services;
        }
    }
}
