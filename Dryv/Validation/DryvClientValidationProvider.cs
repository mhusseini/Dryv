using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Translation;

namespace Dryv.Validation
{
    public class DryvClientValidationProvider : IDryvClientValidationProvider
    {
        private readonly DryvRulesFinder rulesFinder = new DryvRulesFinder();

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
            var validationFunction = translatedRules.Any() ? this.GetValidationFunction(translatedRules) : null;

            return string.IsNullOrWhiteSpace(validationFunction) ? null : new DryvClientValidationItem
            {
                ValidationFunction = validationFunction,
                Key = key,
                ModelType = modelType,
                Property = property,
                ModelPath = modelPath,
            };
        }

        protected virtual string GetValidationFunction(IDictionary<DryvRuleTreeNode, string> translatedRules)
        {
            return $@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f}).call(this, m)"))}; }}";
        }
    }
}