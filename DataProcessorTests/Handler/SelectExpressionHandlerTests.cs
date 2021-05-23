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
        LambdaExpression Build(params string[] select) {
            return new SelectExpressionHandler(typeof(Tuple<string>)).Build(select);
        }

        [Test()]
        public void Select() {
            var expr = Build("Item1");
            Assert.AreEqual(expr.Body.ToString(), "new Item1;String;() {Item1 = IIF((obj == null), null, obj.Item1)}");

        }
    }
}