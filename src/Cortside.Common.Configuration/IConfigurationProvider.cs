using System;

namespace Cortside.Common.Configuration {
    /// <summary>
    /// Summary description for IConfigurationProvider.
    /// </summary>
    public interface IConfigurationProvider {
        Nullable<T> Get<T>(string key) where T : struct;
        string Get(string key);
    }
}
