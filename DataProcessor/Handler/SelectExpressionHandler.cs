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
            var sourceExpr = Expression.Parameter(ItemType, "obj");
            return Expression.Lambda(BuildCore(select, sourceExpr), sourceExpr);
        }

        /// <summary>
        /// To geneate nested select expression from inner class to outer class each by each
        /// </summary>
        /// <param name="fields">An array of string which concat with dot</param>
        /// <param name="sourceExpr"></param>
        /// <returns></returns>
        Expression BuildCore(string[] fields, Expression sourceExpr) {
            var bindings = new List<MemberBinding>();
            //Type dynamicType = sourceExpr.Type;
            var childInstances = new Dictionary<string, Expression>();
            // starts from deepest field
            foreach(var parent in fields.GroupBy(e => e.Split('.', 2).Length).OrderByDescending(e => e.Key)) {
                if(parent.Key > 1) {
                    foreach(var child in parent.GroupBy(e => e.Split('.', 2)[0])) {
                        var splits = child.Select(e => e.Split('.', 2)[1]).ToList();
                        var expr = BuildCore(splits.ToArray(), Expression.PropertyOrField(sourceExpr, child.Key));

                        //var dynamicFields = expr.Type.GetFields().ToDictionary(
                        //e => e.Name,
                        //e => e.FieldType
                        //);
                        //dynamicFields.Add(child.Key, expr.Type);

                        //dynamicType = DynamicTypeUtility.Parse(uuu);
                        bindings.Add(Expression.Bind(
                            sourceExpr.Type.GetProperty(child.Key),
                            ApplyNullGuard(Expression.PropertyOrField(sourceExpr, child.Key), child.Key, expr)
                            ));
                    }
                } else {
                    // bind primitive values 

                    //var dynamicFields = parent.ToDictionary(
                    //    e => e,
                    //    e => sourceExpr.Type.GetProperty(e).PropertyType
                    //    );
                    //dynamicType = DynamicTypeUtility.Parse(dynamicFields);
                    bindings.AddRange(parent.Select(e => Expression.Bind(
                        sourceExpr.Type.GetProperty(e),
                        Expression.PropertyOrField(sourceExpr, e))
                    ));

                }
            }

            // Init member with optional instances of children
            if(childInstances.Count > 0) {
                bindings.AddRange(childInstances.Select(e => Expression.Bind(sourceExpr.Type.GetProperty(e.Key), e.Value)));
            }
            return Expression.MemberInit(Expression.New(sourceExpr.Type), bindings);
        }
    }
}
