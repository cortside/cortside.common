using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cortside.Common.Json.Serialization {
    public class OrderedContractResolver : CamelCasePropertyNamesContractResolver {
        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization) {
            return base.CreateProperties(type, memberSerialization)
                .OrderBy(p => p.Order ?? int.MaxValue)
                .ThenBy(p => p.PropertyName)
                .ToList();
        }
    }
}
