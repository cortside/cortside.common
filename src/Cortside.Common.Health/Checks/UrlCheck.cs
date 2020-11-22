using System;
using System.Threading.Tasks;
using Cortside.Common.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.Common.Health.Checks {
    public class UrlCheck : Check {

        public UrlCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(cache, logger, recorder) { }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            var client = new RestClient();
            var request = new RestRequest(check.Value, Method.GET);
            request.Timeout = (int)TimeSpan.FromSeconds(check.Timeout).TotalMilliseconds;

            var response = await client.ExecuteTaskAsync(request);

            return new ServiceStatusModel() {
                Healthy = response.IsSuccessful,
                Status = response.IsSuccessful ? ServiceStatus.Ok : ServiceStatus.Failure,
                StatusDetail = response.IsSuccessful ? "Successful" : response.ErrorMessage,
                Timestamp = DateTime.UtcNow,
                Required = check.Required
            };
        }
    }
}

