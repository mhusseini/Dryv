using Escape;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Dryv.Tests
{
    [TestClass]
    public class RegularExpressionTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateIsMatch()
        {
            var pattern = @"^\d+$";
            var expression = Expression(m =>
                new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(m.Text)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
            Assert.AreEqual(RegExpFlags.IgnoreCase, regexp?.Flags);
            Assert.AreEqual("test", method.Name);
        }

        [TestMethod]
        public void TranslateMatch()
        {
            var pattern = @"^\d+$";
            var expression = Expression(m =>
                new Regex(pattern).Match(m.Text).Success
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
            var regexp = (method.Object as Literal)?.Value as RegExp;

            Assert.AreEqual(pattern, regexp?.Pattern);
            Assert.AreEqual(RegExpFlags.None, regexp?.Flags);
            Assert.AreEqual("test", method.Name);
        }
    }
}