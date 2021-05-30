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

        public Expression BuildLoadExpression() {
            AddFilter();
            AddSort();
            AddSelect();
            AddGroup();
            AddPaging();
            return Expression;
        }

        public Expression BuildCountExpression() {
            AddFilter();
            AddCount();
            return Expression;
        }

        void AddFilter() {
            if(Context.HasFilter) {
                Expression = QueryableCall(
                    nameof(Queryable.Where),
                    Expression.Quote(
                        new FilterExpressionHandler(GetGenerictTypes().First()).Build(Context.Filter)));
            }
        }

        void AddSort() {
            if(Context.HasSort) {
                Expression = new SortExpressionHandler(GetGenerictTypes().First()).Build(Context.Sort, Expression);
            }
        }

        void AddSelect() {
            if(Context.HasSelect) {
                var type = GetGenerictTypes()[0];
                var expr = new SelectExpressionHandler(GetGenerictTypes().First()).Build(Context.Select);
                Expression = QueryableCall(nameof(Queryable.Select), new Type[] { type, expr.ReturnType }, Expression.Quote(expr));
            }
        }

        void AddGroup() {
            if(Context.HasGroup) {
                Expression = new GroupExpressionHandler(GetGenerictTypes().First()).Build(Context.Group, Expression);
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

        Expression QueryableCall(string method, Expression arg) => QueryableCall(method,  GetGenerictTypes(), arg);


        Type[] GetGenerictTypes() {
            const string queryable1 = "IQueryable`1";
            var type = Expression.Type;

            //if(type.IsInterface && type.Name == queryable1)
            return type.GenericTypeArguments;

            //return type.GetInterface(queryable1).GenericTypeArguments;
        }
    }
}
