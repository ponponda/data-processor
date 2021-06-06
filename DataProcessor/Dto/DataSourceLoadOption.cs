using System.Collections;

namespace DataProcessor.Dto {
    public class DataSourceLoadOption {
        /// <summary>
        /// A filter expression which opperator accepts "=", "!=", ">", "<", ">=", "<=".
        /// </summary>
        public IList Filter { get; set; }
        public string[] Select { get; set; }
        public string[] Group { get; set; }
        public SortingInfo[] Sort { get; set; }
        public SummaryInfo[] Summary { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
