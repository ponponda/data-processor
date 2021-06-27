using DataProcessor.Dto;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Handler {
    class SortExpressionHandler : ExpressionHandler {
        public SortExpressionHandler(Type itemType) : base(itemType) {
        }

        /// <summary>
        /// Generate expression: 
        /// obj.OrderBy[Descending](obj => obj.sortings[0])
        /// .ThenBy[Descending](obj => obj.sortings[1])
        /// </summary>
        /// <param name="sortings"></param>
        /// <param name="sourceExpr">given data source</param>
        /// <returns></returns>
        public Expression Build(Expression sourceExpr, SortingInfo[] sortings) {
            var dataItem = CreateItemParam();

            var doOrder = false;
            foreach(var info in sortings) {
                var memberExpr = Expression.Lambda(Expression.PropertyOrField(dataItem, info.Field), dataItem);
                var method = doOrder ?
                    info.Desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy) :
                    info.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                sourceExpr = QueryableCall(method, sourceExpr, memberExpr);
                doOrder = true;
            }
            return sourceExpr;
        }
    }
}
