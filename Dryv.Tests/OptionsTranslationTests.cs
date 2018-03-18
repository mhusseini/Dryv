using System;
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
        public void ExplicitOptions()
        {
            var expression = TestWithOptionsModel.Rules.PropertyRules.First().Value.First();
            var jsProgram = GetTranslatedAst(expression, null, new object[]
            {
                Options.Create(new TestOptions
                {
                    IsUpperCase = true
                })
            });

            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var literal = (((conditional.Test as BinaryExpression)?.Left as ConditionalExpression)?.Test as Literal)?.Raw;

            Assert.AreEqual("true", literal);
        }

        private class TestWithOptionsModel
        {
            public static readonly DryvRules Rules = DryvRules
                .For<TestWithOptionsModel>()
                .Rule<IOptions<TestOptions>>(
                    m => m.Text,
                    (m, o) => (o.Value.IsUpperCase ? m.Text.ToUpper() : m.Text) == "test"
                        ? DryvResult.Success
                        : "fail");

            [DryvRules]
            public string Text { get; set; }
        }

        private class TestOptions
        {
            public bool IsUpperCase { get; set; }
        }
    }
}