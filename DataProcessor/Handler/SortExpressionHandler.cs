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
        /// <param name="expr"></param>
        /// <returns></returns>
        public Expression Build(SortingInfo[] sortings, Expression expr) {
            var sourceExpr = Expression.Parameter(ItemType, "obj");

            var doOrder = false;
            foreach(var info in sortings) {
                var memberExpr = Expression.Lambda(Expression.PropertyOrField(sourceExpr, info.Field), sourceExpr);
                var method = doOrder ?
                    info.Desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy) :
                    info.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                expr = Expression.Call(
                    typeof(Queryable),
                    method,
                    new Type[] { ItemType, memberExpr.ReturnType },
                    expr,
                    Expression.Quote(memberExpr)
                    );
                doOrder = true;
            }
            return expr;
        }
    }
}
