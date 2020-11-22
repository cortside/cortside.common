using System;
using System.Collections.Generic;
using Cortside.Common.Health.Checks;
using Cortside.Common.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Health {
    public class CheckFactory : ICheckFactory {

        private readonly IMemoryCache cache;
        private readonly ILogger<Check> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IAvailabilityRecorder recorder;
        private readonly IConfiguration configuration;
        private readonly Dictionary<string, Type> checks;

        public CheckFactory(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder, IServiceProvider serviceProvider, IConfiguration configuration) {
            this.cache = cache;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.recorder = recorder;
            this.configuration = configuration;
            checks = new Dictionary<string, Type>() {
                ["url"] = typeof(UrlCheck),
                ["dbcontext"] = typeof(DbContextCheck)
            };
        }

        public ILogger<Check> Logger => logger;

        public IAvailabilityRecorder Recorder => recorder;

        public Check Create(CheckConfiguration check) {
            check.Value = ExpandTemplate(check.Value);

            if (checks.ContainsKey(check.Type)) {
                var type = checks[check.Type];
                var instance = serviceProvider.GetService(type) as Check;
                instance.Initialize(check);
                return instance;
            } else {
                throw new ArgumentException("Invalid Type", check.Type);
            }
        }

        public string ExpandTemplate(string template) {
            if (string.IsNullOrWhiteSpace(template)) {
                return template;
            }

            var start = template.IndexOf("{{");
            if (start >= 0) {
                var end = template.LastIndexOf("}}");
                var key = template.Substring(start + 2, end - start - 2);
                var value = configuration[key];

                if (value != null) {
                    return template.Replace("{{" + key + "}}", value);
                }
            }

            return template;
        }

        public void RegisterCheck(string name, Type type) {
            checks.Add(name, type);
        }
    }
}
