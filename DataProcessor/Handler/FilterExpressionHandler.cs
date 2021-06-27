using DataProcessor.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataProcessor.Handler {
    class FilterExpressionHandler : ExpressionHandler {
        const string
         OPERATION_CONTAINS = "contains",
         OPERATION_NOT_CONTAINS = "notcontains",
         OPERATION_STARTS_WITH = "startswith",
         OPERATION_ENDS_WITH = "endswith";

        public FilterExpressionHandler(Type itemType) : base(itemType) { }

        public Expression Build(Expression sourceExpr, IList filterJson) {
            var dataItem = CreateItemParam();

            return QueryableCall(
                   nameof(Queryable.Where),
                   sourceExpr,
                   Expression.Quote(
                       Expression.Lambda(BuildCore(filterJson, dataItem), dataItem)));
        }

        Expression BuildCore(IList filterJson, ParameterExpression sourceExpr) {
            if(Convert.ToString(filterJson[0]) == "!") {
                return BuildUnary(filterJson, sourceExpr);
            }

            if(IsCriteria(filterJson[0])) {
                return BuildGroup(filterJson, sourceExpr);
            }

            return BuildBinary(filterJson, sourceExpr);
        }

        Expression BuildUnary(IList filterJson, ParameterExpression sourceExpr) => Expression.Not(BuildCore((IList)filterJson[1], sourceExpr));

        Expression BuildGroup(IList filterJson, ParameterExpression sourceExpr) {
            var tempExpressions = new List<Expression>();
            var isAnd = true;
            foreach(var json in filterJson) {
                if(IsCriteria(json)) {
                    var item = json as IList;
                    tempExpressions.Add(BuildCore(item, sourceExpr));
                } else {
                    isAnd = Regex.IsMatch(Convert.ToString(json), "and|&", RegexOptions.IgnorePatternWhitespace);
                }
            }

            if(tempExpressions.Count > 0) {
                Expression result = null;
                var op = isAnd ? ExpressionType.AndAlso : ExpressionType.OrElse;
                foreach(var item in tempExpressions) {
                    if(result == null) result = item;
                    else result = Expression.MakeBinary(op, result, item);
                }

                return result;
            }

            return null;
        }

        Expression BuildBinary(IList filterJson, ParameterExpression sourceExpr) {
            var hasOperation = filterJson.Count > 2;

            var field = Convert.ToString(filterJson[0]);
            var oper = hasOperation ? Convert.ToString(filterJson[1]).ToLower() : "=";
            var value = filterJson[hasOperation ? 2 : 1];
            var isStringOperation = OPERATION_CONTAINS == oper || OPERATION_NOT_CONTAINS == oper || OPERATION_STARTS_WITH == oper || OPERATION_ENDS_WITH == oper;


            var fieldExpr = ApplyNullGuard(sourceExpr, field);
            var valueExpr = Expression.Constant(value, fieldExpr.Type);

            if(isStringOperation) {
                return HandleStringOpertaion(fieldExpr, oper, valueExpr);
            }
            return Expression.MakeBinary(ConvertBinaryOperation(oper), fieldExpr, valueExpr);
        }

        bool IsCriteria(object obj) => obj is IList;

        Expression HandleStringOpertaion(Expression fieldExpr, string oper, Expression valueExpr) {
            var invert = false;
            if(OPERATION_NOT_CONTAINS == oper) {
                oper = OPERATION_CONTAINS;
                invert = true;
            }

            var method = typeof(string).GetMethod(GetStringOperationMethod(oper), new[] { typeof(string) });

            Expression result = Expression.Call(fieldExpr, method, valueExpr);
            if(invert) result = Expression.Not(result);
            return result;
        }

        ExpressionType ConvertBinaryOperation(string oper) {
            switch(oper) {
                case "=":
                    return ExpressionType.Equal;

                case "!=":
                    return ExpressionType.NotEqual;

                case ">":
                    return ExpressionType.GreaterThan;

                case ">=":
                    return ExpressionType.GreaterThanOrEqual;

                case "<":
                    return ExpressionType.LessThan;

                case "<=":
                    return ExpressionType.LessThanOrEqual;
            }

            throw new NotSupportedException();
        }

        string GetStringOperationMethod(string oper) {
            if(oper == OPERATION_STARTS_WITH)
                return nameof(String.StartsWith);

            if(oper == OPERATION_ENDS_WITH)
                return nameof(String.EndsWith);

            return nameof(String.Contains);
        }
    }
}
