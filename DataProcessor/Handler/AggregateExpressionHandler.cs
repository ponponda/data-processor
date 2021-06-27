using DataProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Handler {
    // Used for group summary now
    class AggregateExpressionHandler : ExpressionHandler {
        private IEnumerable<SummaryInfo> Summaries;
        public AggregateExpressionHandler(Type itemType, SummaryInfo[] summaries) : base(itemType) {
            Summaries = summaries;
        }

        /// <summary>
        /// Generate expression:
        /// obj.Sum(sum => sum.[Field])
        /// </summary>
        /// <param name="sourceExpr">given data source</param>
        /// <returns>new Expression[]</returns>
        public Expression Build(ParameterExpression sourceExpr) {
            var list = new List<Expression>();
            if(Summaries != null) {
                foreach(var summary in Summaries) {
                    var dataItem = CreateItemParam(summary.Type);
                    var member = Expression.Lambda(Expression.PropertyOrField(dataItem, summary.Field), dataItem);

                    switch(summary.Type) {
                        case "avg":
                            list.Add(ToObject(EnumerableCall(nameof(Enumerable.Average), new Type[] { ItemType }, sourceExpr, member)));
                            break;
                        case "count":
                            list.Add(ToObject(EnumerableCall(nameof(Enumerable.Count), new Type[] { ItemType }, sourceExpr)));
                            break;
                        case "max":
                            list.Add(ToObject(EnumerableCall(nameof(Enumerable.Max), new Type[] { ItemType, member.ReturnType }, sourceExpr, member))); break;
                        case "min":
                            list.Add(ToObject(EnumerableCall(nameof(Enumerable.Min), new Type[] { ItemType, member.ReturnType }, sourceExpr, member))); break;
                        case "sum":
                            list.Add(ToObject(EnumerableCall(nameof(Enumerable.Sum), new Type[] { ItemType }, sourceExpr, member)));
                            break;
                    }
                }
            }
            return Expression.NewArrayInit(typeof(object), list);
        }

        private Expression ToObject(Expression arg) => Expression.Convert(arg, typeof(object));
    }
}
