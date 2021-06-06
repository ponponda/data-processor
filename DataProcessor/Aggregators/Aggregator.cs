using System;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    abstract class Aggregator<T> {
        protected readonly Expression<Func<T, object>> SelectorExpression;
        public Aggregator(Expression<Func<T, object>> selectorExpression) {
            SelectorExpression = selectorExpression;
        }

        public abstract void Next(T data, string field);
        public abstract object Finish();
    }
}
