using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Utils;

namespace Dryv
{
    public class DryvClientModelValidator : IDryvClientModelValidator
    {
        public const string DataTypeDryAttribute = "data-val-dryv-type";
        public const string DataValAttribute = "data-val";
        public const string DataValDryAttribute = "data-val-dryv";

        public IDictionary<string, string> GetValidationAttributes(
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

            var rules = from rule in modelType.GetRulesForProperty(property, modelPath)
                        where rule.Rule.IsEnabled(services) &&
                              rule.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Client)
                        select rule;

            var translatedRules = rules.Translate(
                services,
                options,
                modelPath,
                modelType);

            return this.CreateValidationAttributes(modelType, property, translatedRules);
        }

        protected virtual IDictionary<string, string> CreateValidationAttributes(
            Type modelType,
            PropertyInfo property,
            IEnumerable<string> translatedRules)
        {
            return new Dictionary<string, string>
            {
                {DataValAttribute, "true"},
                {DataValDryAttribute, $@"[{string.Join(",", translatedRules)}]"},
                {DataTypeDryAttribute, property.PropertyType.GetJavaScriptType()}
            };
        }
    }
}