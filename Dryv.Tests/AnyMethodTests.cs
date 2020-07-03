using System.Linq;
using System.Text.RegularExpressions;
using Dryv.Extensions;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class AnyMethodTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateWithArguments()
        {
            var expression = Expression(m =>
                new TestModelHelper().DoSomething("a", "b")
                    ? "fail"
                    : DryvValidationResult.Success);

            var jsProgram = GetTranslatedAst(expression, new object[] { new AllMethodCallTranslator() });
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);

            var callExpression = conditional?.Test as CallExpression;
            Assert.IsNotNull(callExpression);

            var method = GetMethod(callExpression);
            Assert.AreEqual(method.Name, nameof(TestModelHelper.DoSomething).ToCamelCase());

            Assert.AreEqual(2, callExpression.Arguments.Count());
        }

        [TestMethod]
        public void TranslateWithNoArguments()
        {
            var expression = Expression(m =>
                new TestModelHelper().DoSomething()
                    ? "fail"
                    : DryvValidationResult.Success);

            var jsProgram = GetTranslatedAst(expression, new object[] { new AllMethodCallTranslator() });
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);

            var callExpression = conditional?.Test as CallExpression;
            Assert.IsNotNull(callExpression);

            var method = GetMethod(callExpression);
            Assert.AreEqual(method.Name, nameof(TestModelHelper.DoSomething).ToCamelCase());

            Assert.AreEqual(0, callExpression.Arguments.Count());
        }

        [TestMethod]
        public void TranslateWithRegexMatch()
        {
            var expression = Expression(m =>
                new Regex(@"\d").Match(m.Text) == null
                    ? "fail"
                    : DryvValidationResult.Success);

            var jsProgram = GetTranslatedAst(expression, new object[] { new AllMethodCallTranslator() });
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);

            var binaryExpression = conditional.Test as BinaryExpression;

            var leftMethod = GetMethod(binaryExpression?.Left);
            Assert.AreEqual("match", leftMethod.Name);
        }

        private class RegexMatchTranslator : MethodCallTranslator
        {
            public RegexMatchTranslator()
            {
                this.Supports<Regex>();

                this.AddMethodTranslator(nameof(Regex.Match), context =>
                {
                    context.Translator.Translate(context.Expression.Object, context);
                    context.Writer.Write(".match(");
                    WriteArguments(context.Translator, context.Expression.Arguments, context);
                    context.Writer.Write(")");
                });
            }
        }

        private class TestModelHelper
        {
            public bool DoSomething() => true;

            // ReSharper disable UnusedParameter.Local
            public bool DoSomething(string arg1, string arg2) => true;

            // ReSharper restore UnusedParameter.Local
        }
    }
}