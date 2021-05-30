using DataProcessor.Dto;
using System.Collections;

namespace DataProcessor {
    partial class DataSourceLoadContext {
        readonly DataSourceLoadOption Options;
        public DataSourceLoadContext(DataSourceLoadOption options) {
            Options = options;
        }

        static bool IsEmpty(ICollection data) {
            return data == null || data.Count < 1;
        }
    }

    // filter
    partial class DataSourceLoadContext {
        public IList Filter => Options.Filter;
        public bool HasFilter => Filter != null;
    }

    // group
    partial class DataSourceLoadContext {
        public string[] Group => Options.Group;
        public bool HasGroup => !IsEmpty(Group);
    }

    // paging
    partial class DataSourceLoadContext {
        public int Skip => Options.Skip ?? 0;
        public int Take => Options.Take ?? 0;
        public bool HasPaging => Skip > 0 || Take > 0;
    }

    // seleect
    partial class DataSourceLoadContext {
        public string[] Select => Options.Select;
        public bool HasSelect => !IsEmpty(Select);
    }

    // sort
    partial class DataSourceLoadContext {
        public SortingInfo[] Sort => Options.Sort;
        public bool HasSort => !IsEmpty(Sort);
    }
}
