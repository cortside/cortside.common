using System;
using Cortside.Common.Health.Checks;
using Cortside.Common.Health.Models;
using Microsoft.Extensions.Logging;

namespace Cortside.Common.Health {
    public interface ICheckFactory {

        public ILogger<Check> Logger { get; }

        public IAvailabilityRecorder Recorder { get; }

        public Check Create(CheckConfiguration check);

        public string ExpandTemplate(string template);

        public void RegisterCheck(string name, Type type);
    }
}
