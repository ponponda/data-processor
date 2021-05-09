using System.Collections;

namespace DataProcessor {
    public class DataSourceLoadOption {
        public IList Filter { get; set; }
        public string[] Select { get; set; }
        public string Sort { get; set; } = "Id";
        public bool Descending { get; set; } = false;
        public int? Skip { get; set; } = 0;
        public int? Take { get; set; } = 10;
    }
}
