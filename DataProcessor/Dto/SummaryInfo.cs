using System.Collections;

namespace DataProcessor.Dto {
    public class SummaryInfo {
        public string Field { get; set; }
        /// <summary>
        /// An aggreate function: min, max, sum, avg, count
        /// </summary>
        public string Type { get; set; }
    }
}
