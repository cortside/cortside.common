using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Cortside.Common.Configuration {
    public abstract class ConfigurationProvider : IConfigurationProvider {
        private static IConfigurationProvider instance = null;

        public static IConfigurationProvider Instance {
            get { return instance ??= new AppSettingsProvider(); }
        }

        public static void SetConfigurationProvider(IConfigurationProvider provider) {
            instance = provider;
        }

        protected IConfigurationProvider chainedProvider = null;
        protected NameValueCollection settings = null;

        protected ConfigurationProvider(IConfigurationProvider provider) {
            chainedProvider = provider;
        }

        public Nullable<T> Get<T>(string key) where T : struct {
            T? result = null;
            if (settings.AllKeys.Contains(key)) {
                object safeValue = settings[key];
                if (safeValue != null) {
                    try {
                        Type t = typeof(T);

                        if (t.GetTypeInfo().IsEnum) {
                            if (Enum.TryParse<T>(safeValue.ToString(), out T enumVal)) {
                                safeValue = enumVal;
                            }
                        } else {
                            safeValue = Convert.ChangeType(safeValue, t);
                        }
                    } catch (FormatException) {
                        //change safeValue to null if failing to cast string to type T.  
                        safeValue = null;
                    }
                }
                result = (T?)safeValue;
            }

            if (result == null && chainedProvider != null) {
                result = chainedProvider.Get<T>(key);
            }
            return result;
        }

        public string Get(string key) {
            string setting = settings[key];
            if (string.IsNullOrEmpty(setting) && chainedProvider != null) {
                setting = chainedProvider.Get(key);
            }
            return setting;
        }
    }
}
