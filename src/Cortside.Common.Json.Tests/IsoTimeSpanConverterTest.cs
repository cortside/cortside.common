using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Cortside.Common.Json.Tests {
    public class IsoTimeSpanConverterTest {
        [Fact]
        public void GetSerializeDisbursementParameterTest() {
            TestModel applicationDisbursementParameterModel = new TestModel() {
                Duration = new TimeSpan(1, 2, 3)
            };

            string json = JsonConvert.SerializeObject(applicationDisbursementParameterModel, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { new IsoTimeSpanConverter() } });
            json.ShouldContain("P0Y0M0DT1H2M3S");
        }

        [Fact]
        public void GetDeserializeDisbursementParameterTest() {
            const string json = "{ \"Duration\":\"P0Y0M0DT1H2M3S\" }";
            var model = JsonConvert.DeserializeObject<TestModel>(json, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { new IsoTimeSpanConverter() } });
            model.Duration.ShouldBe(new TimeSpan(1, 2, 3));
        }

        [Fact]
        public void GetDeserializeDisbursementParameterTest2() {
            const string json = "{ \"Duration\":\"2.14:28:13\" }";
            var model = JsonConvert.DeserializeObject<TestModel>(json, new JsonSerializerSettings() { Converters = new List<JsonConverter>() { new IsoTimeSpanConverter() } });
            model.Duration.ShouldBe(new TimeSpan(2, 14, 28, 13));
        }
    }

    internal class TestModel {
        public TimeSpan Duration { get; set; }
    }
}
