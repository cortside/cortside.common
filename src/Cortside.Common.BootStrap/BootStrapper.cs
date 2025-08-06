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
            if (installer == null) {
                throw new ArgumentNullException(nameof(installer), "Installer cannot be null.");
            }
            installers.Add(installer);
        }

        /// <summary>
        /// Installs all of the specified installers, overriding the internal list of installers.
        /// </summary>
        /// <param name="installers">A list of installers to register with the IoC.</param>
        public virtual IServiceProvider InitIoCContainer(params IInstaller[] installers) {
            if (installers == null) {
                throw new ArgumentNullException(nameof(installers), "Installers cannot be null");
            }
            IServiceProvider container = InternalInitialize(installers);
            return container;
        }

        /// <summary>
        /// Installs all the internally specified installers, while adding the [applicationInstaller]
        /// </summary>
        /// <param name="applicationInstaller">The additional installer for the root level application.</param>
        public virtual IServiceProvider InitIoCContainer(IInstaller applicationInstaller) {
            if (applicationInstaller == null) {
                throw new ArgumentNullException(nameof(applicationInstaller), "Application installer cannot be null");
            }
            installers.Add(applicationInstaller);
            return InternalInitialize(installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer() {
            return InternalInitialize(installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer(IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services), "Service collection cannot be null");
            }
            return InternalInitialize(services, installers.ToArray());
        }

        public virtual IServiceProvider InitIoCContainer(IConfigurationBuilder config, IServiceCollection services) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config), "Configuration builder cannot be null");
            }
            return InternalInitialize(config, services, installers.ToArray());
        }
        public virtual IServiceProvider InitIoCContainer(IConfiguration configuration, IServiceCollection services) {
            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
            }
            if (services == null) {
                throw new ArgumentNullException(nameof(services), "Service collection cannot be null");
            }
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
                .AddJsonFile("appsettings.json");
            return InternalInitialize(configuration, services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IConfigurationBuilder config, IServiceCollection services, IInstaller[] installers) {
            var configuration = config.Build();
            return InternalInitialize(configuration, services, installers);
        }

        protected internal virtual IServiceProvider InternalInitialize(IConfiguration configuration, IServiceCollection services, IInstaller[] installers) {
            DI.SetConfiguration(configuration);

            foreach (var i in installers) {
                i.Install(services, configuration);
            }

            services.AddSingleton<IConfiguration>(configuration);
            var serviceProvider = services.BuildServiceProvider();

            DI.SetContainer(serviceProvider);
            return serviceProvider;
        }
    }
}
