using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spring2.Common.BootStrap {

    public interface IInstaller {

	void Install(IServiceCollection services, IConfigurationRoot configuration);
    }
}