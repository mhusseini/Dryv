using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class EnumTranslationTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void EnumsAreSerializedAsConfigured()
        {
            var expression = Expression<Model>(m => m.Prop1 != MyEnum.Two ? null : "fail");
            var translation = Translate<TestModel>(expression);
            var model = @"{prop1:'One'}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script) as string;

            Assert.AreEqual("fail", result);
        }

        private enum MyEnum
        {
            One,
            Two
        }

        private class Model
        {
            public MyEnum Prop1 { get; set; }
        }
    }
}