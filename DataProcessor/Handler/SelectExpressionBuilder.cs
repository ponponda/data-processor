using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Handler {
    public class SelectExpressionBuilder {
        Expression Expression;
        DataSourceLoadOption LoadOptions;
        public SelectExpressionBuilder(Expression expr, DataSourceLoadOption options) {
            Expression = expr;
            LoadOptions = options;
        }

        public Expression Build() {
            return Expression.Empty();
        }
    }
}
