using NUnit.Framework;
using DataProcessor.Handler;
using System.Linq.Expressions;
using System.Linq;
using DataProcessorTests.Dto;
using DataProcessor.Dto;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class SortExpressionHandlerTests : TestExpressionHandler {
        Expression Build(SortingInfo[] sortings) {
            return new SortExpressionHandler(typeof(MockData)).Build(CreateTargetParam<MockData>(), sortings);
        }

        [Test()]
        public void Sort() {
            var expr = Build(new[]{
                new SortingInfo { Field = "IntProp", Desc = false },
                new SortingInfo { Field = "StringProp", Desc = true }
                });
            Assert.AreEqual(expr.ToString(), "obj.OrderBy(obj => obj.IntProp).ThenByDescending(obj => obj.StringProp)");
        }
    }
}