using System;
using System.Collections.Specialized;

namespace Spring2.Common.Configuration {

    /// <summary>
    /// Summary description for IConfigurationProvider.
    /// </summary>
    public interface IConfigurationProvider {
	Nullable<T> Get<T>(String key) where T : struct;
	String Get(String key);
    }
}