using System.Linq;
using Dryv.DependencyInjection;
using Dryv.MethodCallTranslation;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class AnyMethodTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateWithNoArguments()
        {
            var expression = Expression(m =>
                new TestModelHelper().DoSomething()
                    ? "fail"
                    : DryvResult.Success);

            var options = new DryvOptions();
            options.MethodCallTanslators.Clear();
            options.MethodCallTanslators.Add(new AllMethodCallTranslator());

            var jsProgram = GetTranslatedAst(expression, options);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);

            var callExpression = conditional?.Test as CallExpression;
            Assert.IsNotNull(callExpression);

            var method = GetMethod(callExpression);
            Assert.AreEqual(method.Name, nameof(TestModelHelper.DoSomething).ToCamelCase());

            Assert.AreEqual(0, callExpression.Arguments.Count());
        }

        [TestMethod]
        public void TranslateWithArguments()
        {
            var expression = Expression(m =>
                new TestModelHelper().DoSomething("a", "b")
                    ? "fail"
                    : DryvResult.Success);

            var options = new DryvOptions();
            options.MethodCallTanslators.Clear();
            options.MethodCallTanslators.Add(new AllMethodCallTranslator());

            var jsProgram = GetTranslatedAst(expression, options);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);

            var callExpression = conditional?.Test as CallExpression;
            Assert.IsNotNull(callExpression);

            var method = GetMethod(callExpression);
            Assert.AreEqual(method.Name, nameof(TestModelHelper.DoSomething).ToCamelCase());

            Assert.AreEqual(2, callExpression.Arguments.Count());
        }

        private class TestModelHelper
        {
            public bool DoSomething() => true;

            public bool DoSomething(string arg1, string arg2) => true;
        }
    }
}