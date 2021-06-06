using System;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class AvgAggregator<T> : Aggregator<T> {
        CountAggregator<T> Counter;
        SumAggregator<T> Summator;

        public AvgAggregator(Expression<Func<T, object>> expression) : base(expression) {
            Counter = new CountAggregator<T>(expression);
            Summator = new SumAggregator<T>(expression);
        }

        public override void Next(T data, string field) {
            Counter.Next(data, field);
            Summator.Next(data, field);
        }

        public override object Finish() {
            var count = (int)Counter.Finish();
            if(count == 0)
                return null;

            var accumulator = Summator.GetAccumulator();
            accumulator.Divide(count);
            return accumulator.GetValue();
        }
    }
}
