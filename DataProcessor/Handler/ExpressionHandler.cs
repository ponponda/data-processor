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
                falsyExpression ?? Expression.Constant(Utility.TypeUtility.GetDefaultValue(current.Type), current.Type),
                truthyExpression ?? current);
        }

        protected ParameterExpression CreateItemParam(string alias = "obj") => Expression.Parameter(ItemType, alias);

        protected Expression QueryableCall(string method, Expression sourceExpr, Expression arg) {
            var types = new List<Type>();
            types.AddRange(GetGenerictTypes(sourceExpr.Type));
            if(arg.NodeType == ExpressionType.Lambda) {
                types.Add(((LambdaExpression)arg).ReturnType);
            }
            return Expression.Call(typeof(Queryable), method, types.ToArray(), sourceExpr, arg);
        }

        protected Expression EnumerableCall(string method, Expression sourceExpr, Expression arg) {
            var types = new List<Type>();
            types.AddRange(GetGenerictTypes(sourceExpr.Type));
            if(arg.NodeType == ExpressionType.Lambda) {
                types.Add(((LambdaExpression)arg).ReturnType);
            }
            return Expression.Call(typeof(Enumerable), method, types.ToArray(), sourceExpr, arg);
        }

        protected Expression EnumerableCall(string method, Type[] types, Expression sourceExpr) {
            return Expression.Call(typeof(Enumerable), method, types, sourceExpr);
        }

        protected Expression EnumerableCall(string method, Type[] types, Expression sourceExpr, Expression arg) {
            return Expression.Call(typeof(Enumerable), method, types, sourceExpr, arg);
        }

        Type[] GetGenerictTypes(Type type) {
            const string queryable1 = "IQueryable`1";
            const string grouping2 = "IGrouping`2";

            if(type.IsInterface && (type.Name == queryable1 || type.Name == grouping2))
                return type.GenericTypeArguments;

            return type.GetInterface(queryable1).GenericTypeArguments;
        }
    }
}
