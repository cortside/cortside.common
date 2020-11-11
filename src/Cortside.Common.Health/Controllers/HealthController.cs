using System;
using Cortside.Common.Health.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Health.Controllers {

    /// <summary>
    /// Health
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/health")]
    [ApiController]
    [Produces("application/json")]
    public class HealthController : ControllerBase {
        private readonly IMemoryCache cache;
        private readonly ILogger<HealthController> logger;
        private readonly HealthCheckServiceConfiguration config;

        /// <summary>
        /// HealthController constructor
        /// </summary>
        public HealthController(IMemoryCache cache, ILogger<HealthController> logger, HealthCheckServiceConfiguration config) {
            this.cache = cache;
            this.logger = logger;
            this.config = config;
        }

        /// <summary>
        /// Get current health of service
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet()]
        [ProducesResponseType(typeof(HealthModel), 200)]
        [ProducesResponseType(typeof(HealthModel), 503)]
        public IActionResult Get() {
            HealthModel result = cache.Get(config.Name) as HealthModel;
            if (result == null) {
                logger.LogError("Unable to find valid healhcheck in cache", "HealthController");
                result = new HealthModel {
                    Healthy = false,
                    Timestamp = DateTime.UtcNow,
                    Status = ServiceStatus.Failure,
                    StatusDetail = "Status not found"
                };
            }

            if (result.Healthy) {
                return Ok(result);
            } else {
                return StatusCode(503, result);
            }
        }
    }
}
