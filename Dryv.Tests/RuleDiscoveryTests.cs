using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class RuleDiscoveryTests
    {
        private interface IModel
        {
            string Text { get; set; }
        }

        [TestMethod]
        public void FinderulesForBaseClass()
        {
            var model = new Model4();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesForInterface()
        {
            var model = new Model();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesInProperty()
        {
            var model = new Model3();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesInStaticMethods()
        {
            var model = new Model5();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesOnOtherType()
        {
            var model = new Model2();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        private abstract class CommonRules
        {
            public static readonly DryvRules Text = DryvRules
                .For<IModel>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResult.Success
                        : DryvResult.Error("error"));
        }

        private class Model : IModel
        {
            public static DryvRules Rules = DryvRules
                .For<IModel>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResult.Success
                        : DryvResult.Error("error"));

            public string Text { get; set; }
        }

        private class Model2 : IModel
        {
            public static DryvRules Rules = CommonRules.Text;

            public string Text { get; set; }
        }

        private class Model3 : IModel
        {
            public static DryvRules Rules => CommonRules.Text;

            public string Text { get; set; }
        }

        private class Model4 : ModelBase
        {
            public static DryvRules Rules = DryvRules
                .For<ModelBase>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResult.Success
                        : DryvResult.Error("error"));

            public override string Text { get; set; }
        }

        private class Model5 : ModelBase
        {
            public static DryvRules Rules() => DryvRules
                .For<Model5>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResult.Success
                        : DryvResult.Error("error"));
        }

        private abstract class ModelBase
        {
            public virtual string Text { get; set; }
        }
    }
}