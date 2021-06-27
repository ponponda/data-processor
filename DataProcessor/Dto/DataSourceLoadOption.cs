using System.Collections;

namespace DataProcessor.Dto {
    public class DataSourceLoadOption {
        /// <summary>
        /// A filter expression which opperator accepts "=", "!=", ">", "<", ">=", "<=".
        /// </summary>
        public IList Filter { get; set; }
        public string[] Select { get; set; }
        public GroupingInfo[] Group { get; set; }
        public SortingInfo[] Sort { get; set; }
        /// <summary>
        /// A total summary expression
        /// </summary>
        public SummaryInfo[] TotalSummary { get; set; }
        /// <summary>
        /// A group summary expression
        /// </summary>
        public SummaryInfo[] GroupSummary { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
