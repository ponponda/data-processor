using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DataProcessor.Handler.Tests {
    [TestFixture()]
    public class SelectExpressionHandlerTests {
        class MockData {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
            public int? NullableProp { get; set; }
            public DateTime DateProp { get; set; }
        }

        LambdaExpression Build(string[] select) {
            return new SelectExpressionHandler(typeof(MockData)).Build(select);
        }

        [Test()]
        public void Select() {
            var expr = Build(new[] { "IntProp", "StringProp" });
            Assert.AreEqual(expr.Body.ToString(), "new MockData() {IntProp = obj.IntProp, StringProp = obj.StringProp}");
        }
    }
}