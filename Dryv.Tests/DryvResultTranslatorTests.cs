using System.Linq;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class DryvResultTranslatorTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void Fail()
        {
            var expression = Expression(m =>
                m.Text == "test"
                    ? DryvResult.Error("fail")
                    : null);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var properties = (conditional.Consequent as ObjectExpression)?.Properties;
            Assert.IsNotNull(properties);

            var first = (dynamic) properties.First().Value;
            Assert.AreEqual("error", first.Value);

            var second = (dynamic)properties.Last().Value;
            Assert.AreEqual("\"fail\"", second.Value);
        }

        [TestMethod]
        public void Success()
        {
            var expression = Expression(m =>
                m.Text == "test"
                    ? DryvResult.Success
                    : "fail");

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var literal = (conditional.Consequent as Literal)?.Raw;

            Assert.AreEqual("null", literal);
        }
    }
}