using DataProcessor.Dto;
using DataProcessor.Handler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataProcessor.Aggregators {
    class AggregateExecutor<T> : ExpressionHandler {
        IEnumerable Data;
        SummaryInfo[] Summaries;
        Aggregator<T>[] Aggregators;

        public AggregateExecutor(IEnumerable data, SummaryInfo[] summaries) : base(typeof(T)) {
            Data = data;
            Summaries = summaries;
            Aggregators = CreateAggregators(summaries);
        }

        public object[] Execute() {
            if(Aggregators.Length > 0) {
                foreach(var item in Data) {
                    for(var i = 0; i < Aggregators.Length; i++) {
                        Aggregators[i].Next((T)item, Summaries[i].Field);
                    }
                }

                return Aggregators.Select(e => e.Finish()).ToArray();
            } else
                return null;
        }

        private Aggregator<T>[] CreateAggregators(SummaryInfo[] summaries) {
            List<Aggregator<T>> aggregators = new List<Aggregator<T>>();
            var sourceExpr = CreateItemParam();
            foreach(var item in summaries) {
                var u = Expression.Lambda<Func<T, object>>(Expression.Convert(ApplyNullGuard(sourceExpr, item.Field), typeof(object)), sourceExpr);
                switch(item.Type) {
                    case "avg":
                        aggregators.Add(new AvgAggregator<T>(u));
                        break;
                    case "count":
                        aggregators.Add(new CountAggregator<T>(u));
                        break;
                    case "max":
                        aggregators.Add(new MaxAggregator<T>(u));
                        break;
                    case "min":
                        aggregators.Add(new MinAggregator<T>(u));
                        break;
                    case "sum":
                        aggregators.Add(new SumAggregator<T>(u));
                        break;
                    default:
                        continue;
                }
            }

            return aggregators.ToArray();
        }
    }
}
