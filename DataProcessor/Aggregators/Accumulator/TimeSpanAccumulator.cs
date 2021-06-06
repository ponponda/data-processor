using System;
using System.Globalization;

namespace DataProcessor.Aggregators.Accumulators {
    class TimeSpanAccumulator : IAccumulator {
        TimeSpan _value;

        public void Add(object value) {
            _value += (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan), CultureInfo.CurrentCulture);
        }

        public void Divide(int divider) {
            _value = TimeSpan.FromTicks(_value.Ticks / divider);
        }

        public object GetValue() {
            return _value;
        }
    }
}
