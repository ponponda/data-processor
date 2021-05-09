using System.Collections;

namespace DataProcessor {
    partial class DataSourceLoadContext {
        readonly DataSourceLoadOption Option;
        public DataSourceLoadContext(DataSourceLoadOption option) {
            Option = option;
        }
    }

    partial class DataSourceLoadContext {
        public int Skip => Option.Skip ?? 0;
        public int Take => Option.Take ?? 0;
        public bool HasPaging => Skip > 0 || Take > 0;
    }

    partial class DataSourceLoadContext {
        public IList Filter => Option.Filter;
        public bool HasFilter => Filter != null;
    }
}
