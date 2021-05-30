using DataProcessor.Dto;
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
            var expression = new DataSourceExpressionBuilder(DataSource.Expression, Context).BuildLoadExpression();
            var data = DataSource.Provider.CreateQuery(expression);
            var countExpression = new DataSourceExpressionBuilder(DataSource.Expression, Context).BuildCountExpression();
            var count = DataSource.Provider.Execute<int>(countExpression);
            return Task.FromResult(new LoadResult {
                Data = data,
                TotalCount = count
            });
        }
    }
}
