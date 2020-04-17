using System;
using System.Linq;
using System.Reflection;
using Dryv.Cache;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Translation;

namespace Dryv.Validation
{
    public class DryvClientValidationProvider : IDryvClientValidationProvider
    {
        private readonly DryvRulesFinder rulesFinder = new DryvRulesFinder(new InMemoryCache());

        public DryvClientValidationItem GetClientPropertyValidation(
            Type modelType,
            string modelPath,
            PropertyInfo property,
            Func<Type, object> services,
            DryvOptions options)
        {
            if (modelPath == null)
            {
                modelPath = string.Empty;
            }

            var rules = from rule in this.rulesFinder.GetRulesForProperty(modelType, property, modelPath)
                        where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                        select rule;

            var translatedRules = DryvRuleTranslator.Translate(rules, services, options, modelPath, modelType);
            var key = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";
            var code = translatedRules.Any() ? $@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f}).call(this, m)"))}; }}" : null;

            return code == null ? null : new DryvClientValidationItem
            {
                ValidationFunction = code,
                Key = key,
                ModelType = modelType,
                Property = property,
                ModelPath = modelPath,
            };
        }
    }
}