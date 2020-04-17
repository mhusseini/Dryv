using System.Linq;
using Dryv.Cache;
using Dryv.Internal;
using Dryv.RuleDetection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class RuleDiscovery2Tests
    {
        private DryvRulesFinder sut = new DryvRulesFinder(new InMemoryCache());

        private interface IModel
        {
            string Text { get; set; }
        }

        [TestMethod]
        public void FindeRuleOnParentNode()
        {
            var property = typeof(Model9).GetProperty(nameof(Model9.Text));
            var rules = this.sut.GetRulesForProperty(typeof(Model8), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesForInterface()
        {
            var model = new Model();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesInProperty()
        {
            var model = new Model3();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesInStaticMethods()
        {
            var model = new Model5();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FinderulesOnOtherType()
        {
            var model = new Model2();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FindeRuleTree()
        {
            var property = typeof(Model7).GetProperty(nameof(Model7.Text));
            var rules = this.sut.GetRulesForProperty(typeof(Model7), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FindRulesForBaseClass()
        {
            var model = new Model4();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void FindRulesListElements()
        {
            var model = new Model10
            {
                Children = new[]
                {
                    new Model11(),
                    new Model11(),
                }
            };

            var property = typeof(Model11).GetProperty(nameof(Model11.Text));
            var rules = this.sut.GetRulesForProperty(model.GetType(), property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        private abstract class CommonRules
        {
            public static readonly DryvRules Text = DryvRules
                .For<IModel>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));
        }

        private class Model : IModel
        {
            public static DryvRules Rules = DryvRules
                .For<IModel>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));

            [DryvRules]
            public string Text { get; set; }
        }

        private class Model10
        {
            public Model11[] Children { get; set; }
        }

        private class Model11
        {
            public static DryvRules Rules = DryvRules.For<Model11>()
                .Rule(m => m.Text,
                    m => string.IsNullOrWhiteSpace(m.Text)
                        ? "error"
                        : null);

            [DryvRules]
            public string Text { get; set; }
        }

        private class Model2 : IModel
        {
            public static DryvRules Rules = CommonRules.Text;

            [DryvRules]
            public string Text { get; set; }
        }

        private class Model3 : IModel
        {
            public static DryvRules Rules => CommonRules.Text;

            [DryvRules]
            public string Text { get; set; }
        }

        private class Model4 : ModelBase
        {
            public static DryvRules Rules = DryvRules
                .For<ModelBase>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));

            [DryvRules]
            public override string Text { get; set; }
        }

        private class Model5 : ModelBase
        {
            public static DryvRules Rules() => DryvRules
                .For<Model5>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));
        }

        private class Model6
        {
            public static DryvRules Rules = DryvRules
                .For<Model6>()
                .Rule(m => m.Child.Child.Text,
                    m => m.Child.Child.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));

            [DryvRules]
            public Model7 Child { get; set; }
        }

        private class Model7 : IModel
        {
            public static DryvRules Rules = DryvRules
                .For<Model7>()
                .Rule(m => m.Text,
                    m => m.Text != null
                        ? DryvResultMessage.Success
                        : DryvResultMessage.Error("error"));

            public Model7 Child { get; set; }

            [DryvRules]
            public string Text { get; set; }
        }

        private class Model8
        {
            public static DryvRules Rules = DryvRules
                .For<Model9>()
                .Rule(m => m.Text,
                    m => string.IsNullOrWhiteSpace(m.Text)
                        ? "error"
                        : null);

            public Model9 Child { get; set; }
        }

        private class Model9
        {
            [DryvRules]
            public string Text { get; set; }
        }

        private abstract class ModelBase
        {
            [DryvRules]
            public virtual string Text { get; set; }
        }
    }
}