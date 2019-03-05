using System;
using System.Linq;
using System.Reflection;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Translation;

namespace Dryv.Validation
{
    public class DryvClientValidationProvider : IDryvClientValidationProvider
    {
        public DryvClientPropertyValidation GetClientPropertyValidation(
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

            var rules = from rule in DryvReflectionRulesProvider.GetCompiledRulesForProperty(modelType, property, services, modelPath)
                        where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                        select rule;

            var translatedRules = DryvRuleTranslator.Translate(rules, services, options, modelPath, modelType).ToList();
            var key = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";
            var code = translatedRules.Any() ? $@"function(m) {{ return {string.Join("||", translatedRules.Select(f => $"({f})(m)"))}; }}" : null;

            return code == null ? null : new DryvClientPropertyValidation
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