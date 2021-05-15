﻿using DataProcessor.Handler;
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

        public Expression BuildExpression() {
            AddFilter();
            AddSort();
            AddSelect();
            AddPaging();
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
                var type = GetGenerictTypes()[0];
                var method = Context.SortDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                var r = new SortExpressionHandler(GetGenerictTypes().First()).Build(Context.Sort);
                Expression = Expression.Call(
                    typeof(Queryable),
                    method,
                    new[] { type, r.ReturnType },
                    Expression,
                    Expression.Quote(r));
            }
        }

        void AddSelect() {
            if(Context.HasSelect) {
                var type = GetGenerictTypes()[0];
                Expression = Expression.Call(
                    typeof(Queryable),
                    nameof(Queryable.Select),
                    new[] { type, type },
                    Expression,
                    Expression.Quote(new SelectExpressionHandler(GetGenerictTypes().First()).Build(Context.Select)));
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
