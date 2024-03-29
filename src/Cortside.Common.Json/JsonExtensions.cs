﻿using Newtonsoft.Json;

namespace Cortside.Common.Json {
    public static class JsonExtensions {
        /// <summary>
        /// The string representation of null.
        /// </summary>
        private const string Null = "null";

        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Json of any object.</returns>
        public static string ToJson(this object value) {
            if (value == null) {
                return Null;
            }

            string json = JsonConvert.SerializeObject(value);
            return json;
        }
    }
}
