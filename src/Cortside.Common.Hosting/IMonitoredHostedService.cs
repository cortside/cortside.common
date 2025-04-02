using System;

namespace Cortside.Common.Hosting {
    public interface IMonitoredHostedService {
        /// <summary>
        /// Last activity time for purposes of being able to monitor health
        /// </summary>
        DateTime LastActivity { get; }

        /// <summary>
        /// Sleep delay/interval between executions
        /// </summary>
        TimeSpan Interval { get; }
    }
}
