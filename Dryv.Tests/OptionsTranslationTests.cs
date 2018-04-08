using System.Linq;
using Escape.Ast;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class OptionsTranslationTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void InlineOptions()
        {
            var value = "hello";
            var rule = TestWithOptionsModel.Rules.PropertyRules.First();
            var jsProgram = GetTranslatedAst(rule.ValidationExpression, null, new object[]
            {
                Options.Create(new TestOptions
                {
                    Text = value
                })
            });

            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var binaryExpression = conditional.Test as BinaryExpression;
            var literal = binaryExpression?.Right as Literal;

            Assert.AreEqual(value, literal?.Value);
        }

        private class TestOptions
        {
            public string Text { get; set; }
        }

        private abstract class TestWithOptionsModel
        {
            public static readonly DryvRules Rules = DryvRules
                .For<TestWithOptionsModel>()
                .Rule<IOptions<TestOptions>>(
                    m => m.Text,
                    (m, o) => m.Text == o.Value.Text
                        ? DryvResult.Success
                        : "fail");

            [DryvRules]
            public abstract string Text { get; set; }
        }
    }
}