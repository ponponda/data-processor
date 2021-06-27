using NUnit.Framework;
using DataProcessor.Handler;
using System.Linq.Expressions;
using System.Linq;
using DataProcessorTests.Dto;
using DataProcessor.Dto;

namespace DataProcessor.Tests.Handlers {
    public class TestExpressionHandler {
        protected ParameterExpression CreateTargetParam<T>() {
            return Expression.Parameter(typeof(IQueryable<T>), "obj");
        }
    }
}