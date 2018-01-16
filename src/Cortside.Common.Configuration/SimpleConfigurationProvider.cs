using System.Collections.Specialized;

namespace Cortside.Common.Configuration {
    public class SimpleConfigurationProvider : ConfigurationProvider {
        public SimpleConfigurationProvider() : this(null) { }

        public SimpleConfigurationProvider(IConfigurationProvider provider) : base(provider) {
            settings = new NameValueCollection();
        }
    }
}
