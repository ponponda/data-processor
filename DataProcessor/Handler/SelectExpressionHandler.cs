using DataProcessor.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Handler {
    class SelectExpressionHandler : ExpressionHandler {
        public SelectExpressionHandler(Type itemType) : base(itemType) { }


        public LambdaExpression Build(string[] select) {
            var sourceExpr = CreateItemParam();
            return Expression.Lambda(BuildCore(select, sourceExpr), sourceExpr);
        }

        /// <summary>
        /// Generate expression from the inside out
        ///  obj => new [ItemType]() { [Field] = obj.[Field] }
        /// </summary>
        /// <param name="fields">An array of string which concat with dot</param>
        /// <param name="sourceExpr"></param>
        /// <returns></returns>
        Expression BuildCore(string[] fields, Expression sourceExpr) {
            var bindings = new List<MemberBinding>();
            foreach(var parent in fields.GroupBy(e => e.Split('.', 2).Length).OrderByDescending(e => e.Key)) {
                if(parent.Key > 1) {
                    foreach(var child in parent.GroupBy(e => e.Split('.', 2)[0])) {
                        var splits = child.Select(e => e.Split('.', 2)[1]).ToList();
                        var expr = BuildCore(splits.ToArray(), Expression.PropertyOrField(sourceExpr, child.Key));

                        bindings.Add(Expression.Bind(
                            sourceExpr.Type.GetProperty(child.Key),
                            ApplyNullGuard(Expression.PropertyOrField(sourceExpr, child.Key), child.Key, expr)
                            ));
                    }
                } else {
                    bindings.AddRange(parent.Select(e => Expression.Bind(
                        sourceExpr.Type.GetProperty(e),
                        Expression.PropertyOrField(sourceExpr, e))
                    ));

                }
            }

            return Expression.MemberInit(Expression.New(sourceExpr.Type), bindings);
        }
    }
}
