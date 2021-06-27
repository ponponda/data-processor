using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace DataProcessor.Dto {
    public class GroupResult {
        [JsonProperty("key")]
        public object Key { get; set; }
        [JsonProperty("data")]
        public IEnumerable Items { get; set; }
        [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] Summary { get; set; }
    }
}
