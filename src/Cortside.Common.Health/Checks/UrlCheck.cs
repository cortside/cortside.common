using System;
using System.Threading.Tasks;
using Cortside.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Cortside.Health.Checks {
    public class UrlCheck : Check {

        public UrlCheck(CheckConfiguration check, IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(check, cache, logger, recorder) { }

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

