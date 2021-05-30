using Newtonsoft.Json;
using System.Collections;

namespace DataProcessor.Dto {
    public class LoadResult {
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("data")]
        public IEnumerable Data { get; set; }
    }
}
