using System.Collections.Specialized;

namespace Cortside.Common.Configuration {
    public class AppSettingsProvider : ConfigurationProvider {
        public AppSettingsProvider() : this(null, null) { }

        public AppSettingsProvider(string section) : this(section, null) { }

        public AppSettingsProvider(IConfigurationProvider provider) : this(null, provider) { }

        public AppSettingsProvider(string section, IConfigurationProvider provider) : base(provider) {
            if (string.IsNullOrEmpty(section)) {
                settings = base.settings;
            } else {
                settings = new NameValueCollection() {
            { section, provider.Get(section) }
        };
            }
        }
    }
}
