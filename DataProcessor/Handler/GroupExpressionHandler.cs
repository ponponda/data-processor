using DataProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Handler {
    class GroupExpressionHandler : ExpressionHandler {
        public GroupExpressionHandler(Type itemType) : base(itemType) { }

        /// <summary>
        /// Generate expression from the inside out 
        /// obj.GroupBy(g1 => g1.fields[0]).Select(g1 =>
        /// new Group {
        ///     Key = g1.Key,
        ///     Items = g1.GroupBy(g2 => g2.fields[1]).Select(g2 =>
        ///         new Group { Key = g2.Key, Items = g2 })
        /// })
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="expr">data provider expression</param>
        /// <returns></returns>
        // TODO: nested fields, data provider create query broke due to group by turns queryable  into enumerable
        public Expression Build(string[] fields, Expression expr) {
            var index = fields.Length;
            Expression result = null;
            foreach(var f in fields.Reverse()) {
                var memberParam = Expression.Parameter(ItemType, "g" + index);
                var memberLambda = Expression.Lambda(Expression.PropertyOrField(memberParam, f), memberParam);

                var groupType = typeof(IGrouping<,>).MakeGenericType(memberLambda.ReturnType, ItemType);
                var selectParam = Expression.Parameter(groupType, "g" + index);
                var selectLambda = Expression.Lambda(
                    CreateNewGroup(
                        Expression.Convert(Expression.PropertyOrField(selectParam, "Key"), typeof(object)),
                        result ?? selectParam),
                    selectParam
                    );

                // group by
                //if(index == 1) {
                    result = Expression.Call(typeof(Queryable), nameof(Queryable.GroupBy), new Type[] { ItemType, memberLambda.ReturnType }, expr, memberLambda);
                //} else {
                //    result = Expression.Call(typeof(Enumerable), nameof(Enumerable.GroupBy), new Type[] { ItemType, memberLambda.ReturnType }, Expression.Parameter(groupType, "g" + --index), memberLambda);
                //}

                // select to group result
                result = Expression.Call(typeof(Queryable), nameof(Queryable.Select), new Type[] { result.Type.GenericTypeArguments[0], selectLambda.ReturnType }, result, selectLambda);
            }

            return result;
        }

        Expression CreateNewGroup(params Expression[] expr) {
            var bindings = new List<MemberBinding>();

            bindings.Add(Expression.Bind(typeof(GroupResult).GetProperty("Key"), expr[0]));
            bindings.Add(Expression.Bind(typeof(GroupResult).GetProperty("Items"), expr[1]));

            return Expression.MemberInit(Expression.New(typeof(GroupResult)), bindings);
        }
    }
}
