using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cortside.Common.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cortside.Common.Health.Checks {
    public abstract class Check {

        protected CheckConfiguration check;
        protected readonly IMemoryCache cache;
        protected readonly ILogger<Check> logger;
        protected readonly IAvailabilityRecorder recorder;
        protected readonly Availability availability = new Availability();

        protected Check(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) {
            this.cache = cache;
            this.logger = logger;
            this.recorder = recorder;
        }

        internal void Initialize(CheckConfiguration check) {
            this.check = check;

            // fix up and make sure that interval and cache duration are set
            if (check.Interval == 0 && check.CacheDuration == 0) {
                check.Interval = 30;
            }
            if (check.Interval == 0 && check.CacheDuration > 0) {
                check.Interval = check.CacheDuration;
            }
            if (check.CacheDuration < check.Interval) {
                check.CacheDuration = check.Interval * 2;
            }

            logger.LogInformation($"Initializing {check.Name} check of type {this.GetType().Name} with interval of {check.Interval} and cache duration of {check.CacheDuration}");
        }

        public string Name => check.Name;
        public ServiceStatusModel Status => cache.Get<ServiceStatusModel>(Name);

        protected void UpdateAvailability(bool healthy, long elapsedMilliseconds) {
            availability.Count += 1;
            availability.Success += healthy ? 1 : 0;
            availability.Failure += healthy ? 0 : 1;
            availability.Uptime = availability.Success * 100.0 / availability.Count;
            availability.TotalDuration += elapsedMilliseconds;
            availability.AverageDuration = availability.TotalDuration / Convert.ToDouble(availability.Count);
        }

        public async Task InternalExecuteAsync() {
            logger.LogInformation($"Checking status of {Name}");

            var item = cache.Get<ServiceStatusModel>(Name);
            var age = item != null ? (DateTime.UtcNow - item.Timestamp).TotalSeconds : int.MaxValue;
            if (age >= check.Interval) {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                ServiceStatusModel serviceStatusModel;
                try {
                    serviceStatusModel = await ExecuteAsync();
                } catch (Exception ex) {
                    serviceStatusModel = new ServiceStatusModel() {
                        Healthy = false,
                        Timestamp = DateTime.UtcNow,
                        Status = ServiceStatus.Failure,
                        StatusDetail = ex.Message
                    };
                }

                stopwatch.Stop();
                UpdateAvailability(serviceStatusModel.Healthy, stopwatch.ElapsedMilliseconds);
                serviceStatusModel.Availability = availability;
                recorder.RecordAvailability(Name, stopwatch.Elapsed, serviceStatusModel.Healthy, JsonConvert.SerializeObject(serviceStatusModel));

                // Store it in cache
                cache.Set(Name, serviceStatusModel, DateTimeOffset.Now.AddSeconds(check.CacheDuration));
            }
        }

        public abstract Task<ServiceStatusModel> ExecuteAsync();
    }
}
