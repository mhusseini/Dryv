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
        public void FindeRulesForInterface()
        {
            var model = new Model();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FindeRulesOnOtherType()
        {
            var model = new Model2();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FindeRulesInProperty()
        {
            var model = new Model3();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(property);

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
                        : DryvResult.Fail("error"));
        }

        private class Model : IModel
        {
            public static DryvRules Rules = DryvRules
                .For<IModel>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResult.Success
                        : DryvResult.Fail("error"));

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
    }
}