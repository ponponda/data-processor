using DataProcessor.Handler;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor {
    class DataSourceExpressionBuilder {
        Expression Expression;
        DataSourceLoadContext Context;
        public DataSourceExpressionBuilder(Expression expression, DataSourceLoadContext context) {
            Expression = expression;
            Context = context;
        }

        public Expression BuildExpression() {
            AddFilter();
            AddPaging();
            return Expression;
        }
        
        void AddFilter() {
            if(Context.HasFilter) {
                Expression = QueryableCall(nameof(Queryable.Where), Expression.Quote(new FilterExpressionHandler(GetGenerictTypes().First()).Build(Context.Filter)));
            }
        }

        void AddPaging() {
            if(Context.Skip > 0) 
                Expression = QueryableCall(nameof(Queryable.Skip), Expression.Constant(Context.Skip));
            if(Context.Take > 0) 
                Expression = QueryableCall(nameof(Queryable.Take), Expression.Constant(Context.Take));
        }

        Expression QueryableCall(string method, Expression arg) => Expression.Call(typeof(Queryable), method, GetGenerictTypes(), Expression, arg);

        Type[] GetGenerictTypes() {
            const string queryable1 = "IQueryable`1";
            var type = Expression.Type;

            //if(type.IsInterface && type.Name == queryable1)
                return type.GenericTypeArguments;

            //return type.GetInterface(queryable1).GenericTypeArguments;
        }
    }
}
