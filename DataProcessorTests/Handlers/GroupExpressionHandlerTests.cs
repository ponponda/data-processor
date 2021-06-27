using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Linq;
using System.Linq.Expressions;
using DataProcessorTests.Dto;
using DataProcessor.Dto;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class GroupExpressionHandlerTests : TestExpressionHandler {
        Expression Build(GroupingInfo[] fields, SummaryInfo[] groupSummaries = null) {
            return new GroupExpressionHandler(typeof(MockData), fields, groupSummaries).Build(CreateTargetParam<MockData>());
        }

        [Test()]
        public void Group() {
            var expr = Build(new[] { new GroupingInfo { Field = "IntProp" } });
            Assert.AreEqual(expr.ToString(), "obj.GroupBy(g0 => g0.IntProp).Select(g0 => new GroupResult() {Key = Convert(g0.Key, Object), Items = g0, Summary = new [] {}})");
        }

        [Test()]
        public void GroupSummary() {
            var expr = Build(new[] { new GroupingInfo { Field = "IntProp" } }, new SummaryInfo[] {
                //new SummaryInfo { Field =  "IntProp", Type = "avg" },
                new SummaryInfo { Field =  "IntProp", Type = "count" },
                //new SummaryInfo { Field =  "IntProp", Type = "max" },
                //new SummaryInfo { Field =  "IntProp", Type = "min" },
                new SummaryInfo { Field =  "IntProp", Type = "sum" },
            });
            Assert.AreEqual(expr.ToString(), "obj.GroupBy(g0 => g0.IntProp).Select(g0 => new GroupResult() {Key = Convert(g0.Key, Object), Items = g0, Summary = new [] {Convert(g0.Count(), Object), Convert(g0.Sum(sum => sum.IntProp), Object)}})");
        }
    }
}