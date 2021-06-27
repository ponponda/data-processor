using NUnit.Framework;
using DataProcessor.Handler;
using System.Collections;
using System.Linq.Expressions;
using DataProcessorTests.Dto;
using System.Linq;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class FilterExpressionHandlerTests : TestExpressionHandler {
        Expression Build(IList json) {
            return new FilterExpressionHandler(typeof(MockData)).Build(CreateTargetParam<MockData>(), json);
        }

        [Test()]
        public void ExplicitEqual() {
            var expr = Build(new object[] { "IntProp", "=", 2 });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => (IIF((obj == null), 0, obj.IntProp) == 2))");
        }

        [Test()]
        public void ImplicitEqual() {
            var expr = Build(new object[] { "IntProp", 2 });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => (IIF((obj == null), 0, obj.IntProp) == 2))");
        }

        [Test()]
        public void NotEqual() {
            var expr = Build(new object[] { "IntProp", "!=", 2 });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => (IIF((obj == null), 0, obj.IntProp) != 2))");
        }

        [Test()]
        public void NullableEqual() {
            var expr = Build(new object[] { "NullableProp", 2 });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => (IIF((obj == null), null, obj.NullableProp) == 2))");
        }

        [Test()]
        public void StringContains() {
            var expr = Build(new object[] { "StringProp", "contains", "Test" });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => IIF((obj == null), null, obj.StringProp).Contains(\"Test\"))");
        }

        [Test()]
        public void StringNotContains() {
            var expr = Build(new object[] { "StringProp", "notcontains", "Test" });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => Not(IIF((obj == null), null, obj.StringProp).Contains(\"Test\")))");
        }

        [Test()]
        public void StringStartWith() {
            var expr = Build(new object[] { "StringProp", "startswith", "Test" });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => IIF((obj == null), null, obj.StringProp).StartsWith(\"Test\"))");
        }

        [Test()]
        public void StringEndWith() {
            var expr = Build(new object[] { "StringProp", "endswith", "Test" });
            Assert.AreEqual(expr.ToString(), "obj.Where(obj => IIF((obj == null), null, obj.StringProp).EndsWith(\"Test\"))");
        }
    }
}