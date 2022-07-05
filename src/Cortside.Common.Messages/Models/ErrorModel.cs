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
        public string Property { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
    }
}
