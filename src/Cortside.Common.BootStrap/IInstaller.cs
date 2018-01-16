using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.Common.BootStrap {

    public interface IInstaller {

        void Install(IServiceCollection services, IConfigurationRoot configuration);
    }
}
