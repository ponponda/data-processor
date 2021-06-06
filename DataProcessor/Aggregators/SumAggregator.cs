using DataProcessor.Aggregators.Accumulators;
using System;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class SumAggregator<T> : Aggregator<T> {
        private IAccumulator _accumulator;
        public SumAggregator(Expression<Func<T, object>> expression) : base(expression) { 
        }

        public override void Next(T data, string field) {
            var value = SelectorExpression.Compile()(data);
            if(value != null) {
                if(_accumulator == null) _accumulator = NewAccumulator(value.GetType());
                _accumulator.Add(value);
            }
        }

        public override object Finish() => _accumulator?.GetValue();

        public IAccumulator GetAccumulator() => _accumulator;

        private IAccumulator NewAccumulator(Type type) {
            if(type == typeof(double)) {
                return new DoubleAccumulator();
            } 
            if(type == typeof(TimeSpan)) {
                return new TimeSpanAccumulator();
            }
            return new DecimalAccumulator();
        }
    }
}
