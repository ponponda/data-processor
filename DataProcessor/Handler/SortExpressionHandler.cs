using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Handler {
    public class SortExpressionHandler {
        readonly Type ItemType;
        public SortExpressionHandler(Type type) {
            ItemType = type;
        }

        public LambdaExpression Build(string sort) {
            var sourceExpr = Expression.Parameter(ItemType, "obj");
            return Expression.Lambda(BuildCore(sort, sourceExpr), sourceExpr);
        }

        Expression BuildCore(string sort, ParameterExpression sourceExpr) {
            return Expression.PropertyOrField(sourceExpr, sort);
        }
    }
}
