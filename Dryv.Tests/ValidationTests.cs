using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public async Task ValidateChildList()
        {
            var m = new ModelParent
            {
                Children =
                {
                    new ModelChild
                    {
                        Children = new List<ModelGrandChild>
                        {
                            new ModelGrandChild(),
                            new ModelGrandChild(),
                            new ModelGrandChild()
                        }
                    }
                }
            };

            var options = new DryvOptions();
            var treeBuilder = new ModelTreeBuilder();
            var compiler = new DryvCompiler();
            var translator = CreateTranslator(options);
            var ruleFinder = new DryvRuleFinder(treeBuilder, compiler, translator, Array.Empty<IDryvRuleAnnotator>(), options);

            var validator = new DryvValidator(ruleFinder, options);

            var result = await validator.Validate(m, Activator.CreateInstance);
            
            Assert.IsTrue(result.ContainsKey("children[0].name"));
            Assert.IsTrue(result.ContainsKey("children[0].children[0].name"));
            Assert.IsTrue(result.ContainsKey("children[0].children[1].name"));
            Assert.IsTrue(result.ContainsKey("children[0].children[2].name"));
        }

        private static JavaScriptTranslator CreateTranslator(DryvOptions options)
        {
            var methodCallTranslators = new Collection<IDryvMethodCallTranslator>
            {
                new RegexTranslator(),
                new DryvValidationResultTranslator(),
                new StringTranslator(),
                new EnumerableTranslator()
            };

            var customTranslators = new Collection<IDryvCustomTranslator>
            {
                new RegexTranslator(),
                new DryvValidationResultTranslator(),
                new ObjectTranslator()
            };

            return new JavaScriptTranslator(customTranslators, methodCallTranslators, options);
        }

        private class ModelParent
        {
            private static DryvRules Rules = DryvRules.For<ModelParent>()
                .Rule(m => m.Children,
                    m => m.Children != null && m.Children.Any()
                        ? null
                        : "Must contain elements.");

            public List<ModelChild> Children { get; set; } = new List<ModelChild>();
        }

        private class ModelChild
        {
            private static DryvRules Rules = DryvRules.For<ModelChild>()
                .Rule(m => m.Name,
                    m => !string.IsNullOrWhiteSpace(m.Name)
                        ? null
                        : "Must have name");

            public string Name { get; set; }

            public List<ModelGrandChild> Children { get; set; } = new List<ModelGrandChild>();
        }


        private class ModelGrandChild
        {
            private static DryvRules Rules = DryvRules.For<ModelGrandChild>()
                .Rule(m => m.Name,
                    m => !string.IsNullOrWhiteSpace(m.Name)
                        ? null
                        : "Must have name");

            public string Name { get; set; }
        }
    }
}