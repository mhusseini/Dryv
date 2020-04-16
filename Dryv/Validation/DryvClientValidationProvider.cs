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

            var rules = from rule in DryvReflectionRulesProvider.GetCompiledRulesForProperty(modelType, property, services, modelPath)
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

        public IEnumerable<DryvClientValidationItem> GetClientPropertyValidationGroups(
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

            var ruleGroup = from rule in DryvReflectionRulesProvider.GetCompiledRulesForProperty(modelType, property, services, modelPath)
                            where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                            group rule by rule.Rule.GroupName;

            foreach (var rules in ruleGroup)
            {
                var translatedRules = DryvRuleTranslator.Translate(rules, services, options, modelPath, modelType);
                var key = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";
                var code = translatedRules.Any() ? $@"function(m) {{ return {string.Join("||", translatedRules.Values.Select(f => $"({f}).call(this, m)"))}; }}" : null;

                if (code == null)
                {
                    continue;
                }

                yield return new DryvClientValidationItem
                {
                    GroupName = rules.Key,
                    ValidationFunction = code,
                    Key = key,
                    ModelType = modelType,
                    Property = property,
                    ModelPath = modelPath,
                };
            }
        }
    }
}