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
    public class SortExpressionHandlerTests {
        class MockData {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
            public int? NullableProp { get; set; }
            public DateTime DateProp { get; set; }
        }

        LambdaExpression Build(string sort) {
            return new SortExpressionHandler(typeof(MockData)).Build(sort);
        }

        [Test()]
        public void Sort() {
            var expr = Build("IntProp");
            Assert.AreEqual(expr.Body.ToString(), "obj.IntProp");
        }
    }
}