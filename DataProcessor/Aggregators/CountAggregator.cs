using System;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class CountAggregator<T> : Aggregator<T> {
        private int _count = 0;
        public CountAggregator(Expression<Func<T, object>> expression) : base(expression) { }

        public override void Next(T data, string field) {
            if(SelectorExpression.Compile()(data) != null)
                _count++;
        }

        public override object Finish() => _count;
    }
}
