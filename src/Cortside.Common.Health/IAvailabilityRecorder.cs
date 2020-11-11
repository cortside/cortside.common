using System;

namespace Cortside.Health {
    public interface IAvailabilityRecorder {
        void RecordAvailability(string service, TimeSpan duration, bool healthy, string message);
    }
}
