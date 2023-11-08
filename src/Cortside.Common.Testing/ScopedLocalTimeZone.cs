#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

using System;
using System.Reflection;

namespace Cortside.Common.Testing {
    /// <summary>
    /// ScopedLocalTimeZone
    /// </summary>
    /// <remarks>using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("UTC+12"))) {</remarks>
    public class ScopedLocalTimeZone : IDisposable {
        private readonly TimeZoneInfo originalLocalTimeZoneInfo;

        private static void SetLocalTimeZone(TimeZoneInfo timeZoneInfo) {
            var info = typeof(TimeZoneInfo).GetField("s_cachedData", BindingFlags.NonPublic | BindingFlags.Static);
            object cachedData = info.GetValue(null);

            var field = cachedData?.GetType().GetField("_localTimeZone", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            field?.SetValue(cachedData, timeZoneInfo);
        }

        public ScopedLocalTimeZone(TimeZoneInfo timeZoneInfo) {
            originalLocalTimeZoneInfo = TimeZoneInfo.Local;
            SetLocalTimeZone(timeZoneInfo);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                TimeZoneInfo.ClearCachedData();
                SetLocalTimeZone(originalLocalTimeZoneInfo);
            }
        }
    }
}
