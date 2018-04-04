using System;
using System.Linq;
using System.Linq.Expressions;
using Jurassic;
using Jurassic.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class EnumerbaleTests : JavascriptTranslatorTestsBase
    {
        [TestMethod]
        public void TranslateDefaultIfEmpty()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.IntItems.DefaultIfEmpty());
            var translation = Translate(expression);
            var model = @"{intItems:[]}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script) as ArrayInstance;

            Assert.IsNotNull(result);
            Assert.AreEqual((uint)1, result.Length);
            Assert.AreEqual(0, result.ElementValues.First());
        }

        [TestMethod]
        public void TranslateCount()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Count());
            var translation = Translate(expression);
            var model = @"{items:['y', 'x', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TranslateCountWithFilter()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Count(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'xx', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TranslateElementAtOrDefault()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.ElementAtOrDefault(3));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(Null.Value, result);
        }

        [TestMethod]
        public void TranslateAll()
        {
            var expression = Expression(m => m.Items.All(s => s.StartsWith("x")) ? null : "fail");
            var translation = Translate(expression);
            var model = @"{items:['x1', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var result = engine.Evaluate($"({translation})({model})");

            Assert.AreEqual(Jurassic.Null.Value, result);
        }

        [TestMethod]
        public void TranslateAny()
        {
            var expression = Expression(m => m.Items.Any(s => s.StartsWith("x")) ? null : "fail");
            var translation = Translate(expression);
            var model = @"{items:['y', 'x']}";
            var engine = new Jurassic.ScriptEngine();
            var result = engine.Evaluate($"({translation})({model})");

            Assert.AreEqual(Jurassic.Null.Value, result);
        }

        [TestMethod]
        public void TranslateContains()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Contains("x2"));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TranslateElementAt()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.ElementAt(2));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("z", result);
        }

        [TestMethod]
        public void TranslateFirst()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.First(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("x2", result);
        }

        [TestMethod]
        public void TranslateFirstOrDefault()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.FirstOrDefault(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(Null.Value, result);
        }

        [TestMethod]
        public void TranslateLast()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Last(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2', 'x3']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("x3", result);
        }

        [TestMethod]
        public void TranslateLastOrDefault()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.LastOrDefault(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual(Null.Value, result);
        }

        [TestMethod]
        public void TranslateMax()
        {
            var expression = Expression(m => m.Items.Max(i => i.Length).ToString());
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("2", result);
        }

        [TestMethod]
        public void TranslateMin()
        {
            var expression = Expression(m => m.Items.Min(i => i.Length).ToString());
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void TranslateNotAll()
        {
            var expression = Expression(m => m.Items.All(s => s.StartsWith("x")) ? null : "fail");
            var translation = Translate(expression);
            var model = @"{items:['x1', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var result = engine.Evaluate($"({translation})({model})");

            Assert.AreEqual("fail", result);
        }

        [TestMethod]
        public void TranslateNotAny()
        {
            var expression = Expression(m => m.Items.Any(s => s.StartsWith("x")) ? null : "fail");
            var translation = Translate(expression);
            var model = @"{items:['y', 'z']}";
            var engine = new Jurassic.ScriptEngine();
            var result = engine.Evaluate($"({translation})({model})");

            Assert.AreEqual("fail", result);
        }

        [TestMethod]
        public void TranslateSelect()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Select(i => i.Length));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script) as ArrayInstance;

            Assert.IsNotNull(result);
            Assert.AreEqual((uint)2, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void TranslateSum()
        {
            var expression = Expression(m => m.Items.Sum(i => i.Length).ToString());
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script);

            Assert.AreEqual("3", result);
        }

        [TestMethod]
        public void TranslateWhere()
        {
            var expression = (Expression<Func<TestModel, object>>)(m => m.Items.Where(i => i.Length > 1));
            var translation = Translate(expression);
            var model = @"{items:['y', 'x2']}";
            var engine = new Jurassic.ScriptEngine();
            var script = $"({translation})({model})";
            var result = engine.Evaluate(script) as ArrayInstance;

            Assert.IsNotNull(result);
            Assert.AreEqual((uint)1, result.Length);
            Assert.AreEqual("x2", result.ElementValues.First());
        }
    }
}