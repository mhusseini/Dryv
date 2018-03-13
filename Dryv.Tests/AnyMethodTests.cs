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

            var jsProgram = GetTranslatedAst(expression);
            var conditional = GetBodyExpression<ConditionalExpression>(jsProgram);
            var method = GetMethod(conditional?.Test);
        }

        private class TestModelHelper
        {
            public bool DoSomething() => true;

            public bool DoSomething(string arg1, string arg2) => true;
        }
    }
}