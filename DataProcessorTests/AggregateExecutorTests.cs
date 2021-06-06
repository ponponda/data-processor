using NUnit.Framework;
using DataProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessor.Aggregators;
using DataProcessorTests.Dto;

namespace DataProcessor.Tests {
    [TestFixture()]
    public class AggregateExecutorTests {
        List<MockData> source = new List<MockData> {
            new MockData { IntProp = 1, StringProp = "Mock A" },
            new MockData { IntProp = 2, StringProp = "Mock A" }
        };

        [Test()]
        public void Execute() {
            var summaries = new SummaryInfo[] {
                new SummaryInfo{ Field = "IntProp", Type = "avg" },
            };
            var result = new AggregateExecutor<MockData>(source, summaries).Execute();
            Assert.AreEqual(1.5, result[0]);
        }
    }
}