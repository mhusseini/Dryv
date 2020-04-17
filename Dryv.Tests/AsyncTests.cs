using System.Linq;
using System.Threading.Tasks;
using Dryv.Cache;
using Dryv.Compilation;
using Dryv.Extensions;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class AsyncTests
    {
        [TestMethod]
        public async Task Asnyc_Validation_Correctly_Run()
        {
            var validator = new DryvValidator(new DryvRulesFinder(new InMemoryCache()), new DryvServerRuleEvaluator());
            var model = new Model();
            var errors = await validator.ValidateAsync(model);

            Assert.IsTrue(errors.Any(e => e.Message.Any(vr => vr.IsError())));
        }

        private class Model
        {
            public static DryvRules<Model> Rules = DryvRules.For<Model>()
                .ServerRule(m => m.Name,
                    m => Task.FromResult(m.Name == null ? 0 : m.Name.Length).ContinueWith(t => t.Result == 0 ? "failed" : DryvResultMessage.Success));

            [DryvRules]
            public string Name { get; set; }
        }
    }
}