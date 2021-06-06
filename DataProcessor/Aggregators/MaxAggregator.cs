using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class MaxAggregator<T> : Aggregator<T> {
        private object _max = null;
        public MaxAggregator(Expression<Func<T, object>> expression) : base(expression) { }

        public override void Next(T data, string field) {
            var value = SelectorExpression.Compile()(data);

            if(value is IComparable) {
                if(_max == null || Comparer<object>.Default.Compare(value, _max) > 0)
                    _max = value;
            }
        }

        public override object Finish() => _max;
    }
}
