using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Rework.RuleDetection;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Rework
{
    public class DryvTranslator
    {
        private readonly RuleFinder ruleFinder;

        internal DryvTranslator(RuleFinder ruleFinder)
        {
            this.ruleFinder = ruleFinder;
        }

        public TranslatedExpressions GetClientValidation(Type modelType, Func<Type, object> serviceProvider)
        {
            var validationRules = this.ruleFinder.FindValidationRulesInTree(modelType, RuleType.Default);
            var clientValidation = validationRules.Select(r => Translate(serviceProvider, r));

            var disablingRules = this.ruleFinder.FindValidationRulesInTree(modelType, RuleType.Disabling);
            var clientDisablers = disablingRules.Select(r => Translate(serviceProvider, r));

            return new TranslatedExpressions
            {
                ValidationFunctions = clientValidation.ToDictionary(i => i.Key, i => i.Value),
                DisablingFunctions = clientDisablers.ToDictionary(i => i.Key, i => i.Value),
            };
        }

        private static KeyValuePair<string, string> Translate(Func<Type, object> serviceProvider, DryvCompiledRule r)
        {
            var services = r.PreevaluationOptionTypes.Select(serviceProvider);
            var arguments = new object[] { null }.Union(services).ToArray();
            var code = r.TranslatedValidationExpression(_ => null, arguments);

            return new KeyValuePair<string, string>(r.ModelPath, code);
        }

        public class TranslatedExpressions
        {
            public IDictionary<string, string> ValidationFunctions { get; internal set; }
            public IDictionary<string, string> DisablingFunctions { get; internal set; }
        }
    }
}