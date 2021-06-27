using DataProcessor.Handler;
using System;
using System.Collections.Generic;
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

        public Expression BuildLoadExpression(bool paginate) {
            AddFilter();
            AddSort();
            AddSelect();
            if(paginate)
                AddPaging();
            return Expression;
        }

        public Expression BuildCountExpression() {
            AddFilter();
            AddCount();
            return Expression;
        }

        public  Expression BuildGroupExpression() {
            AddGroup();
            return Expression;
        }

        void AddFilter() {
            if(Context.HasFilter) {
                Expression = new FilterExpressionHandler(GetItemType()).Build(Expression, Context.Filter);
            }
        }

        void AddSort() {
            if(Context.HasSort) {
                Expression = new SortExpressionHandler(GetItemType()).Build(Expression, Context.Sort);
            }
        }

        void AddSelect() {
            if(Context.HasSelect) {
                Expression = new SelectExpressionHandler(GetItemType()).Build(Expression, Context.Select);
            }
        }

        void AddGroup() {
            if(Context.HasGroup) {
                Expression = new GroupExpressionHandler(GetItemType(), Context.Group, Context.GroupSummary).Build(Expression);
            }
        }

        void AddPaging() {
            if(Context.Skip > 0)
                Expression = QueryableCall(nameof(Queryable.Skip), Expression.Constant(Context.Skip));
            if(Context.Take > 0)
                Expression = QueryableCall(nameof(Queryable.Take), Expression.Constant(Context.Take));
        }

        void AddCount() {
            Expression = QueryableCall(nameof(Queryable.Count));
        }

        Expression QueryableCall(string method) => Expression.Call(typeof(Queryable), method, GetGenerictTypes(), Expression);

        Expression QueryableCall(string method, Type[] types, Expression arg) => Expression.Call(typeof(Queryable), method, types, Expression, arg);

        Expression QueryableCall(string method, Expression arg) => QueryableCall(method, GetGenerictTypes(), arg);


        Type[] GetGenerictTypes() {
            const string queryable1 = "IQueryable`1";
            var type = Expression.Type;

            if(type.IsInterface && type.Name == queryable1)
                return type.GenericTypeArguments;

            return type.GetInterface(queryable1).GenericTypeArguments;
        }

        Type GetItemType() => GetGenerictTypes().First();
    }
}
