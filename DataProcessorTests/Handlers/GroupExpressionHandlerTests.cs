using NUnit.Framework;
using DataProcessor.Handler;
using System;
using System.Linq;
using System.Linq.Expressions;
using DataProcessorTests.Dto;

namespace DataProcessor.Tests.Handlers {
    [TestFixture()]
    public class GroupExpressionHandlerTests {
        ParameterExpression CreateTargetParam<T>() {
            return Expression.Parameter(typeof(IQueryable<T>), "obj");
        }

        Expression Build(string[] fields) {
            return new GroupExpressionHandler(typeof(MockData)).Build(fields, CreateTargetParam<MockData>());
        }

        [Test()]
        public void Group() {
            var expr = Build(new[] { "IntProp" });
            Assert.AreEqual(expr.ToString(), "obj.GroupBy(g1 => g1.IntProp).Select(g1 => new GroupResult() {Key = Convert(g1.Key, Object), Items = g1})");
        }
    }
}