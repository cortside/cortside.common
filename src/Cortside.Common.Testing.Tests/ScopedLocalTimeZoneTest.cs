using System;
using Xunit;

namespace Cortside.Common.Testing.Tests {
    public class ScopedLocalTimeZoneTest {
        [Fact]
        public void UseScopedLocalTimeZone() {
            using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("UTC+12"))) {
                var localDateTime = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Local);
                var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

                Assert.Equal("UTC+12", TimeZoneInfo.Local.Id);
                Assert.Equal(utcDateTime.AddHours(12), localDateTime);
            }
        }
    }
}
