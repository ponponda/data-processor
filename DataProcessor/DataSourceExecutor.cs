using DataProcessor.Aggregators;
using DataProcessor.Dto;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataProcessor {
    class DataSourceExecutor<T> {
        readonly IQueryable DataSource;
        readonly DataSourceLoadContext Context;
        readonly CancellationToken CancellationToken;
        public DataSourceExecutor(IQueryable<T> dataSource, DataSourceLoadOption option, CancellationToken ct) {
            DataSource = dataSource;
            Context = new DataSourceLoadContext(option);
            CancellationToken = ct;
        }

        public Task<LoadResult> ExecuteAsync() {
            var result = new LoadResult();
            var deferPaging = Context.HasSummary;
            var expression = new DataSourceExpressionBuilder(DataSource.Expression, Context).BuildLoadExpression(!deferPaging);
            if(Context.HasGroup) {
                var data = DataSource.Provider.CreateQuery<GroupResult>(expression).AsEnumerable();
                result.Data = data;
            } else {
                var data = DataSource.Provider.CreateQuery<T>(expression).AsEnumerable();
                result.TotalCount = ExecTotalCount(data);
                result.Summary = ExecSummary(data);
                result.Data = Paginate(data);
                CancellationToken.ThrowIfCancellationRequested();
            }

            return Task.FromResult(result);
        }


        private int ExecTotalCount(IEnumerable data) {
            return data.Cast<object>().Count();
        }

        // TODO: Use expression instead
        private object[] ExecSummary(IEnumerable data) {
            return Context.HasSummary ?
                new AggregateExecutor<T>(data, Context.Summary).Execute() :
                null;
        }

        private IEnumerable Paginate(IEnumerable data) {
            if(Context.Skip < 1 && Context.Take < 1)
                return data;

            return data.Cast<object>().Skip(Context.Skip).Take(Context.Take);
        }
    }
}
