using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Linq.Expressions;
using DataProcessorTests.Dto;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class SelectExpressionHandlerTests : TestExpressionHandler {
        Expression Build(params string[] select) {
            return new SelectExpressionHandler(typeof(MockData)).Build(CreateTargetParam<MockData>(), select);
        }

        [Test()]
        public void Select() {
            var expr = Build("IntProp");
            Assert.AreEqual(expr.ToString(), "obj.Select(obj => new MockData() {IntProp = obj.IntProp})");

        }
    }
}