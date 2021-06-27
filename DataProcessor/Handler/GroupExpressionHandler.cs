using DataProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Handler {
    class GroupExpressionHandler : ExpressionHandler {
        private string[] Fields;
        private SummaryInfo[] GroupSummaries = new SummaryInfo[0];
        private List<ParameterExpression> ParameterExpressions = new List<ParameterExpression>();

        public GroupExpressionHandler(Type itemType, string[] fields) : base(itemType) {
            Fields = fields;

            // predefined available parameters
            var dataItem = CreateItemParam();
            for(var i = 0; i < Fields.Length; i++) {
                var f = Fields[Fields.Length - 1 - i];
                var member = Expression.PropertyOrField(dataItem, f);
                var groupType = typeof(IGrouping<,>).MakeGenericType(member.Type, ItemType);
                ParameterExpressions.Add(Expression.Parameter(groupType, "g" + i));
            }
        }

        public GroupExpressionHandler(Type itemType, string[] fields, SummaryInfo[] groupSummaries) : this(itemType, fields) {
            GroupSummaries = groupSummaries;
        }

        /// <summary>
        /// Generate expression from the inside out 
        /// obj.GroupBy(g1 => g1.fields[0]).Select(g1 =>
        /// new Group {
        ///     Key = g1.Key,
        ///     Items = g1.GroupBy(g2 => g2.fields[1]).Select(g2 =>
        ///         new Group { Key = g2.Key, Items = g2 })
        /// })
        /// </summary>
        /// <param name="sourceExpr">given data source</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Expression Build(Expression sourceExpr) {
            var index = 0;
            Expression result = null;
            foreach(var f in Fields.Reverse()) {
                var dataItem = CreateItemParam("g" + index);
                var member = Expression.Lambda(Expression.PropertyOrField(dataItem, f), dataItem);

                var parameter = ParameterExpressions.ElementAt(index);

                // group result
                var groupResult = BuildGroup(parameter, result);

                index++;
                var last = index == Fields.Length;
                // group
                if(last) {
                    result = EnumerableCall(nameof(Enumerable.GroupBy), new Type[] { ItemType, member.ReturnType }, sourceExpr, member);
                } else {
                    parameter = ParameterExpressions.ElementAt(index);
                    result = EnumerableCall(nameof(Enumerable.GroupBy), new Type[] { ItemType, member.ReturnType }, parameter, member);
                }

                result = EnumerableCall(nameof(Enumerable.Select), new Type[] { result.Type.GenericTypeArguments[0], groupResult.ReturnType }, result, groupResult);
            }

            return result;
        }

        private LambdaExpression BuildGroup(ParameterExpression parameter, Expression arg = null) {
            var summaryExpr = new AggregateExpressionHandler(ItemType, GroupSummaries).Build(parameter);
            return Expression.Lambda(ToGroupResult(
                 Expression.Convert(Expression.PropertyOrField(parameter, "Key"), typeof(object)),
                 arg ?? parameter,
                 summaryExpr
                ),
            parameter
            );
        }

        private Expression ToGroupResult(params Expression[] expr) {
            var bindings = new List<MemberBinding>();

            bindings.Add(Expression.Bind(typeof(GroupResult).GetProperty("Key"), expr[0]));
            bindings.Add(Expression.Bind(typeof(GroupResult).GetProperty("Items"), expr[1]));
            bindings.Add(Expression.Bind(typeof(GroupResult).GetProperty("Summary"), expr[2]));

            return Expression.MemberInit(Expression.New(typeof(GroupResult)), bindings);
        }
    }
}
