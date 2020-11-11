using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cortside.Health.Models {

    /// <summary>
    /// Build
    /// </summary>
    public class BuildModel {
        /// <summary>
        /// Build date
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Build version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Build tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Build suffix
        /// </summary>
        public string Suffix { get; set; }
    }
}
