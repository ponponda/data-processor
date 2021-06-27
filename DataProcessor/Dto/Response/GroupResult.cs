using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace DataProcessor.Dto {
    public class GroupResult {
        [JsonProperty("key")]
        public object Key { get; set; }
        [JsonProperty("data")]
        public IEnumerable Items { get; set; }
        /// <summary>
        /// The count of items in the group.
        /// </summary>
        [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Count { get; set; }

        [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] Summary { get; set; }
    }
}
