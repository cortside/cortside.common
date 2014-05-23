using System.Configuration;

namespace Spring2.Common.Configuration {
    public class AppSettingsProvider : ConfigurationProvider {
	public AppSettingsProvider() : this(null) { }

	public AppSettingsProvider(IConfigurationProvider provider) : base(provider) {
	    settings = ConfigurationManager.AppSettings;
	}
    }
}
