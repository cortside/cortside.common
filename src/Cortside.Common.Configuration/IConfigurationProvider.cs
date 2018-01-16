using System;

namespace Cortside.Common.Configuration {

    /// <summary>
    /// Summary description for IConfigurationProvider.
    /// </summary>
    public interface IConfigurationProvider {
        Nullable<T> Get<T>(String key) where T : struct;
        String Get(String key);
    }
}
