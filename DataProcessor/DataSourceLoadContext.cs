using System.Collections;

namespace DataProcessor {
    partial class DataSourceLoadContext {
        readonly DataSourceLoadOption Options;
        public DataSourceLoadContext(DataSourceLoadOption options) {
            Options = options;
        }
    }

    partial class DataSourceLoadContext {
        public IList Filter => Options.Filter;
        public bool HasFilter => Filter != null;
    }

    partial class DataSourceLoadContext {
        public string Sort => Options.Sort;
        public bool SortDescending => Options.SortDescending;
        public bool HasSort => !string.IsNullOrEmpty(Sort);
    }

    partial class DataSourceLoadContext {
        public string[] Select => Options.Select;
        public bool HasSelect => Select != null && Select.Length > 0;
    }

    partial class DataSourceLoadContext {
        public int Skip => Options.Skip ?? 0;
        public int Take => Options.Take ?? 0;
        public bool HasPaging => Skip > 0 || Take > 0;
    }
}
