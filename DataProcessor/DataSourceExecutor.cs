using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataProcessor {
    class DataSourceExecutor<T> {
        readonly IQueryable DataSource;
        readonly DataSourceLoadContext Context;
        public DataSourceExecutor(IQueryable<T> dataSource, DataSourceLoadOption option, CancellationToken ct) {
            DataSource = dataSource;
            Context = new DataSourceLoadContext(option);
        }

        public Task<LoadResult> ExecuteAsync() {
            var expression = new DataSourceExpressionBuilder(DataSource.Expression, Context).BuildExpression();
            var data = DataSource.Provider.CreateQuery<T>(expression);
            return Task.FromResult(new LoadResult {
                Data = data,
                TotalCount = data.Count()
            });
        }
    }
}
