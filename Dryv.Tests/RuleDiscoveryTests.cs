using System;
using System.Linq;
using Escape.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class RuleDiscoveryTests
    {
        [TestMethod]
        public void FindeRulesForInterface()
        {
            var model = new Model();
            var property = model.GetType().GetProperty(nameof(model.Text));
            var rules = RulesFinder.GetRulesForProperty(property);

            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        private interface IModel
        {
            string Text { get; set; }
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
    }
}