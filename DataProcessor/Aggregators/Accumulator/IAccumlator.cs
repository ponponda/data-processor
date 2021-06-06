
namespace DataProcessor.Aggregators.Accumulators {
    interface IAccumulator {
        void Add(object value);
        void Divide(int divider);
        object GetValue();
    }
}
