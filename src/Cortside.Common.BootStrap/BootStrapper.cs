using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.Common.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.BootStrap {
    public class BootStrapper {
        protected IList<IInstaller> installers;

        public BootStrapper() {
            installers = new List<IInstaller>();
        }

        public virtual void AddInstaller(IInstaller installer) {
            installers.Add(installer);
        }

        /// <summary>
        /// Installs all of the specified installers, overriding the internal list of installers.
        /// </summary>
        /// <param name="installers">A list of installers to register with the IoC.</param>
        public virtual IServiceProvider InitIoCContainer(params IInstaller[] installers) {
            IServiceProvider container = InternalInitialize(installers);
            return container;
        }

        /// <summary>
        /// Installs all of the interally specified installers, while adding the [applicationInstaller]
        /// </summary>
        /// <param name="applicationInstaller">The additional installer for the root level application.</param>
        public virtual IServiceProvider InitIoCContainer(IInstaller applicationInstaller) {
            installers.Add(applicationInstaller);
            return InternalInitialize(installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer() {
            return InternalInitialize(installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer(IServiceCollection services) {
            return InternalInitialize(services, installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer(IConfigurationBuilder config, IServiceCollection services) {
            return InternalInitialize(config, services, installers.ToArray());
        }
        public virtual IServiceProvider InitIoCContainer(IConfigurationRoot configuration, IServiceCollection services) {
            return InternalInitialize(configuration, services, installers.ToArray());
        }

        protected internal virtual IServiceProvider InternalInitialize(IInstaller[] installers) {
            var services = new ServiceCollection().AddOptions();
            return InternalInitialize(services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IConfigurationBuilder config, IInstaller[] installers) {
            var services = new ServiceCollection().AddOptions();
            return InternalInitialize(config, services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IServiceCollection services, IInstaller[] installers) {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("../config.json");
            return InternalInitialize(configuration, services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IConfigurationBuilder config, IServiceCollection services, IInstaller[] installers) {
            var configuration = config.Build();
            return InternalInitialize(configuration, services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IConfigurationRoot configuration, IServiceCollection services, IInstaller[] installers) {
            DI.SetConfiguration(configuration);

            foreach (var i in installers) {
                i.Install(services, configuration);
            }

            services.AddSingleton<IConfigurationRoot>(configuration);
            var serviceProvider = services.BuildServiceProvider();

            DI.SetContainer(serviceProvider);
            return serviceProvider;
        }
    }
}
