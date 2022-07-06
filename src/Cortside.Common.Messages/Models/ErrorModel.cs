using System;
using Newtonsoft.Json;

namespace Cortside.Common.Messages.Models {
    /// <summary>
    /// Error model
    /// </summary>
    public class ErrorModel {
        /// <summary>
        /// Error model
        /// </summary>
        public ErrorModel() { }

        /// <summary>
        /// Error model
        /// </summary>
        public ErrorModel(string type, string property, string message) {
            Type = type;
            Property = property;
            Message = message;
        }

        /// <summary>
        /// Error model
        /// </summary>
        public ErrorModel(Exception ex) {
            Type = ex.GetType().Name;
            Message = ex.Message;
            Exception = ex;
        }

        /// <summary>
        /// Error type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Error field
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Property { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error exception
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Exception Exception { get; set; }
    }
}
