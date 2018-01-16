using System;
using System.Collections.Specialized;

namespace Cortside.Common.Configuration {
    public class AppSettingsProvider : ConfigurationProvider {
        public AppSettingsProvider() : this(null, null) { }

        public AppSettingsProvider(String section) : this(section, null) { }

        public AppSettingsProvider(IConfigurationProvider provider) : this(null, provider) { }

        public AppSettingsProvider(String section, IConfigurationProvider provider) : base(provider) {
            if (String.IsNullOrEmpty(section)) {
                settings = base.settings;
            } else {
                settings = new NameValueCollection() {
            { section, provider.Get(section) }
        };
            }
        }
    }
}
