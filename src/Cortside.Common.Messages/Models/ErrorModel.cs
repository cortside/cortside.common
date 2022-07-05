using System.Collections.Generic;

namespace Cortside.Common.Messages.Models {
    /// <summary>
    /// Error model
    /// </summary>
    public class ErrorModel {
        /// <summary>
        /// Error type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Error field
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Fields
        /// </summary>
        public List<ErrorModel> Fields { get; set; } = new List<ErrorModel>();
    }
}
