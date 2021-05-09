using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;

namespace DataProcessor.Handler.Tests {
    [TestFixture()]
    public class FilterExpressionHandlerTests {
        class MockData {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
            public int? NullableProp { get; set; }
            public DateTime DateProp { get; set; }
        }

        LambdaExpression Build(IList json) {
            return new FilterExpressionHandler(typeof(MockData)).Build(json);
        }

        [Test()]
        public void ExplicitEqual() {
            var expr = Build(new object[] { "IntProp", "=", 2 });
            Assert.AreEqual(expr.Body.ToString(), "(obj.IntProp == 2)");
        }

        [Test()]
        public void ImplicitEqual() {
            var expr = Build(new object[] { "IntProp", 2 });
            Assert.AreEqual(expr.Body.ToString(), "(obj.IntProp == 2)");
        }

        [Test()]
        public void NotEqual() {
            var expr = Build(new object[] { "IntProp", "!=", 2 });
            Assert.AreEqual(expr.Body.ToString(), "(obj.IntProp != 2)");
        }

        [Test()]
        public void NullableEqual() {
            var expr = Build(new object[] { "NullableProp", 2 });
            Assert.AreEqual(expr.Body.ToString(), "(obj.NullableProp == 2)");
        }

        [Test()]
        public void StringContains() {
            var expr = Build(new object[] { "StringProp", "contains", "Test" });
            Assert.AreEqual(expr.Body.ToString(), "obj.StringProp.Contains(\"Test\")");
        }

        [Test()]
        public void StringNotContains() {
            var expr = Build(new object[] { "StringProp", "notcontains", "Test" });
            Assert.AreEqual(expr.Body.ToString(), "Not(obj.StringProp.Contains(\"Test\"))");
        }

        [Test()]
        public void StringStartWith() {
            var expr = Build(new object[] { "StringProp", "startswith", "Test" });
            Assert.AreEqual(expr.Body.ToString(), "obj.StringProp.StartsWith(\"Test\")");
        }

        [Test()]
        public void StringEndWith() {
            var expr = Build(new object[] { "StringProp", "endswith", "Test" });
            Assert.AreEqual(expr.Body.ToString(), "obj.StringProp.EndsWith(\"Test\")");
        }
    }
}