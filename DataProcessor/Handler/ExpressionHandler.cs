using DataProcessor.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Handler {
    abstract class ExpressionHandler {
        protected readonly Type ItemType;
        public ExpressionHandler(Type itemType) {
            ItemType = itemType;
        }

        /// <summary>
        /// To generate condition, obj == null || obj.Prop == null ? default(obj.Prop.Var) : obj.Prop.Var
        /// </summary>
        /// <param name="sourceExpr"></param>
        /// <param name="field">field key which concat with dot</param>
        /// <param name="truthyExpression"></param>
        /// <param name="falsyExpression"></param>
        /// <returns></returns>
        protected internal Expression ApplyNullGuard(Expression sourceExpr, string field, Expression truthyExpression = null, Expression falsyExpression = null) {
            Expression current = sourceExpr, condition = null;

            var nullConst = Expression.Constant(null);
            foreach(var f in field.Split('.')) {
                var isNull = Expression.Equal(current, nullConst);
                condition = condition != null ? Expression.OrElse(condition, isNull) : isNull;
                current = Expression.PropertyOrField(current, f);
            }
            return Expression.Condition(
                condition,
                falsyExpression ?? Expression.Constant(TypeUtility.GetDefaultValue(current.Type), current.Type),
                truthyExpression ?? current);
        }

        protected ParameterExpression CreateItemParam() => Expression.Parameter(ItemType, "obj");
    }
}
