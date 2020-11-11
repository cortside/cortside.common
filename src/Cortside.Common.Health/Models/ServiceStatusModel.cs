using System;

namespace Cortside.Common.Health.Models {
    /// <summary>
    /// Service status
    /// </summary>
    public class ServiceStatusModel {
        /// <summary>
        /// Is the service healhy or not.  Healthy service may included degraded functionality.
        /// </summary>
        public bool Healthy { get; set; }

        public ServiceStatus Status { get; set; }

        public string StatusDetail { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Required { get; set; }

        public Availability Availability { get; set; }
    }
}
