using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dryv.Tests
{
    [TestClass]
    public class StringTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateStringIgnoreCase()
        {
            var expression = Expression(m =>
                m.Text.Equals("Oscorp", StringComparison.OrdinalIgnoreCase)
                    ? "fail"
                    : DryvResult.Success);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            Assert.AreEqual("toLowerCase", leftMethod.Name);

            var rightMethod = GetMethod(binaryExpression?.Right);
            Assert.AreEqual("toLowerCase", rightMethod.Name);
        }
    }
}