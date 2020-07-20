using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers.Translation;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;

namespace Dryv.Tests
{
    [TestClass]
    public class ModelTreeBuilderTests
    {
        [TestMethod]
        public void Test()
        {
            var finder = new ModelTreeBuilder();
            var tree = finder.Build(typeof(ParentModel));
        }
    }

    [TestClass]
    public class RuleMatchingTests
    {
        [TestMethod]
        public async Task Test()
        {

            var m = new ParentModel
            {
                Child1 = new ChildModel
                {
                    Name = "b"
                }
            };

            var translatorProvider = new TranslatorProvider();

            translatorProvider.MethodCallTranslators.Add(new RegexTranslator());
            translatorProvider.MethodCallTranslators.Add(new DryvValidationResultTranslator());
            translatorProvider.MethodCallTranslators.Add(new StringTranslator());
            translatorProvider.MethodCallTranslators.Add(new EnumerableTranslator());
            translatorProvider.GenericTranslators.Add(new RegexTranslator());
            translatorProvider.GenericTranslators.Add(new DryvValidationResultTranslator());
            translatorProvider.GenericTranslators.Add(new ObjectTranslator());

            var options = new DryvOptions();
            var treeBuilder = new ModelTreeBuilder();
            var compiler = new DryvCompiler();
            var javaScriptTranslator = new JavaScriptTranslator(translatorProvider, options);
            var ruleFinder = new DryvRuleFinder(treeBuilder, compiler, javaScriptTranslator, options);
            var modelType = typeof(ParentModel);
            
            var validator = new DryvValidator(ruleFinder, options);
            var errors = await validator.Validate(m, t => new Thingy());
            var translator = new DryvTranslator(ruleFinder);
            var code = translator.TranslateValidationRules(m.GetType(), t => null);
        }
    }

    internal class ParentModel
    {
        //private static readonly DryvRules Rules = DryvRules.For<ParentModel>()
        //    .Rule(m => m.Child1.Name, m => m.Child1 != null && m.Child1.Name != "a" ? null : "error")
        //    .Rule(m => m.Child2.Id, m => m.Child2 != null && m.Child2.Id != "a" ? null : "error");
        public string Id { get; set; }

        public ChildModel Child1 { get; set; }
        public ChildModel Child2 { get; set; }
    }

    internal class Thingy
    {
        public Task<bool> IsValid(string text) => Task.FromResult(false);
    }

    internal class ChildModel
    {
        private static readonly DryvRules Rules = DryvRules.For<ChildModel>()
            .Rule(m => m.Name, m => m != null && m.Name != "b" ? null : "error")
            .Rule(m => m.Id, m => m != null && m.Id != "b" ? null : "error");

        public string Id { get; set; }
        public ChildModel Child3 { get; set; }

        public string Name { get; set; }
    }
}