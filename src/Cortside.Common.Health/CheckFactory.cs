using System;
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

        public CheckFactory(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder, IServiceProvider serviceProvider, IConfiguration configuration) {
            this.cache = cache;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.recorder = recorder;
            this.configuration = configuration;
        }

        public ILogger<Check> Logger => logger;

        public IAvailabilityRecorder Recorder => recorder;

        public Check Create(CheckConfiguration check) {
            check.Value = ExpandTemplate(check.Value);
            switch (check.Type) {
                case "url":
                    return new UrlCheck(check, cache, logger, recorder);
                case "dbcontext":
                    return new DbContextCheck(check, cache, logger, serviceProvider, recorder);
                default:
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
    }
}
