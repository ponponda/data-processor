using Newtonsoft.Json;
using System.Collections;

namespace DataProcessor.Dto {
    public class GroupResult {
        [JsonProperty("key")]
        public object Key { get; set; }
        [JsonProperty("data")]
        public IEnumerable Items { get; set; }
    }
}
