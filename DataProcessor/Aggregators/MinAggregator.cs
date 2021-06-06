using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class MinAggregator<T> : Aggregator<T> {
        private object _min = null;
        public MinAggregator(Expression<Func<T, object>> expression) : base(expression) { }

        public override void Next(T data, string field) {
            var value = SelectorExpression.Compile()(data);

            if(value is IComparable) {
                if(_min == null || Comparer<object>.Default.Compare(value, _min) < 0)
                    _min = value;
            }
        }

        public override object Finish() => _min;
    }
}
