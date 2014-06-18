using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Spring2.Common.Configuration {
    public class AppSettingsProvider : ConfigurationProvider {
	public AppSettingsProvider() : this(null, null) { }

	public AppSettingsProvider(String section) : this(section, null) { }

	public AppSettingsProvider(IConfigurationProvider provider) : this(null, provider) { }

	public AppSettingsProvider(String section, IConfigurationProvider provider) : base(provider) {
	    if (String.IsNullOrEmpty(section)) {
		settings = ConfigurationManager.AppSettings;
	    }
	    else {
		settings = ConfigurationManager.GetSection(section) as NameValueCollection;
	    }
	}
    }
}
