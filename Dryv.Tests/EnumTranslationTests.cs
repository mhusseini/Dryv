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
            var expression = Expression<Model>(m => m.Prop1 == MyEnum.One ? "fail" : null);
            var translation = Translate<Model>(expression);
            var model = @"{prop1:'One'}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"(({translation})({model}) || {{}}).text";
            var result = engine.Evaluate(script);

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