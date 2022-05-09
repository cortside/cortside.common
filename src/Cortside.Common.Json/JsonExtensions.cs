using Newtonsoft.Json;

namespace Cortside.Common.Json {
    public static class JsonExtensions {
        /// <summary>
        /// The string representation of null.
        /// </summary>
        private static readonly string Null = "null";

        /// <summary>
        /// The string representation of exception.
        /// </summary>
        private static readonly string Exception = "Exception";

        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Json of any object.</returns>
        public static string ToJson(this object value) {
            if (value == null)
                return Null;

            string json = JsonConvert.SerializeObject(value);
            return json;
        }
    }
}
