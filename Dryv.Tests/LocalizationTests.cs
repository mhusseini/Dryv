using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class LocalizationTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateLocalizationString()
        {
            var expression = Expression(m =>
                m.Text == null
                    ? LocalizedStrings.String1
                    : null);

            var jsProgram = GetTranslatedAst(expression);
            var conditional = (dynamic)GetBodyExpression<ConditionalExpression>(jsProgram);
            var value = conditional.Consequent.Value;

            Assert.AreEqual(LocalizedStrings.String1, value);
        }
    }
}