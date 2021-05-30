using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Linq.Expressions;
using DataProcessorTests.Dto;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class SelectExpressionHandlerTests {
        LambdaExpression Build(params string[] select) {
            return new SelectExpressionHandler(typeof(MockData)).Build(select);
        }

        [Test()]
        public void Select() {
            var expr = Build("IntProp");
            Assert.AreEqual(expr.Body.ToString(), "new MockData() {IntProp = obj.IntProp}");

        }
    }
}