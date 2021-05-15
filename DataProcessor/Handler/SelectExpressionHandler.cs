using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Handler {
    public class SelectExpressionHandler {
        readonly Type ItemType;
        public SelectExpressionHandler(Type type) {
            ItemType = type;
        }

        public LambdaExpression Build(string[] select) {
            var sourceExpr = Expression.Parameter(ItemType, "obj");
            return Expression.Lambda(BuildCore(select, sourceExpr), sourceExpr);
        }

        MemberInitExpression BuildCore(string[] select, ParameterExpression sourceExpr) {
            var bindings = new List<MemberAssignment>();
            foreach(var item in select) {
                bindings.Add(Expression.Bind(ItemType.GetProperty(item), Expression.PropertyOrField(sourceExpr, item)));
            }
            var newInstance = Expression.MemberInit(
                Expression.New(ItemType),
                bindings
                );
            return newInstance;
        }
    }
}
