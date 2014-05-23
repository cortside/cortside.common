using System.Collections.Specialized;

namespace Spring2.Common.Configuration {
    public class SimpleConfigurationProvider : ConfigurationProvider {
	public SimpleConfigurationProvider() : this(null) { }

	public SimpleConfigurationProvider(IConfigurationProvider provider) : base(provider) {
	    settings = new NameValueCollection();
	}
    }
}
