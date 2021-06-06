using System;
using System.Globalization;

namespace DataProcessor.Aggregators.Accumulators {
    class DecimalAccumulator : IAccumulator {
        decimal _value;

        public void Add(object value) {
            _value += Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }

        public void Divide(int divider) {
            _value /= divider;
        }

        public object GetValue() {
            return _value;
        }
    }
}
