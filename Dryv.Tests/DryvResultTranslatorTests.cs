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
            var literal = (conditional.Consequent as Literal)?.Raw;

            Assert.AreEqual("\"fail\"", literal);
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