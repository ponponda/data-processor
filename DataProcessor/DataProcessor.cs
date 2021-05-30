using DataProcessor.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataProcessor {
    public class DataProcessor {
        public static LoadResult Load<T>(IEnumerable<T> dataSource, DataSourceLoadOption option) => Load(dataSource.AsQueryable(), option);

        public static LoadResult Load<T>(IQueryable<T> dataSource, DataSourceLoadOption option) => LoadAsync(dataSource.AsQueryable(), option).GetAwaiter().GetResult();

        private static Task<LoadResult> LoadAsync<T>(IQueryable<T> dataSource, DataSourceLoadOption option, CancellationToken ct = default(CancellationToken)) =>
            new DataSourceExecutor<T>(dataSource, option, ct).ExecuteAsync();
    }
}
