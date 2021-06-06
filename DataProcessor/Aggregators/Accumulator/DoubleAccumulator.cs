using System;
using System.Globalization;

namespace DataProcessor.Aggregators.Accumulators {
    class DoubleAccumulator : IAccumulator {
        double _value;

        public void Add(object value) {
            _value += Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public void Divide(int divider) {
            _value /= divider;
        }

        public object GetValue() {
            return _value;
        }
    }
}
