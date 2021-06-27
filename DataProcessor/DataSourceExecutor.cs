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

            CancellationToken.ThrowIfCancellationRequested();
            var data = DataSource.Provider.CreateQuery<T>(expression).AsEnumerable();
            result.TotalCount = ExecTotalCount(data);
            result.Summary = ExecSummary(data);
            result.Data = Paginate(data);

            if(Context.HasGroup) {
                expression = new DataSourceExpressionBuilder(DataSource.Expression, Context).BuildGroupExpression();
                CancellationToken.ThrowIfCancellationRequested();

                result.Data = Paginate(DataSource.Provider.Execute<IEnumerable<GroupResult>>(expression));
            }

            return Task.FromResult(result);
        }


        private int ExecTotalCount(IEnumerable data) {
            return data.Cast<object>().Count();
        }

        private object[] ExecSummary(IEnumerable data) {
            return Context.HasTotalSummary ?
                new AggregateExecutor<T>(data, Context.TotalSummary).Execute() :
                null;
        }

        private IEnumerable Paginate(IEnumerable data) {
            if(Context.Skip < 1 && Context.Take < 1)
                return data;

            return data.Cast<object>().Skip(Context.Skip).Take(Context.Take);
        }
    }
}
