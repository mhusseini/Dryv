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

        public ClientModelElementValidation GetValidationAttributes(
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
                modelType).ToList();

            var name = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";

            return new ClientModelElementValidation
            {
                ElementAttribute = this.CreateValidationAttributes(modelType, property, name),
                ValidationCode = translatedRules.Any() ? $"[{string.Join(",", translatedRules)}]" : null,
                Name = name
            };
        }

        protected virtual IDictionary<string, string> CreateValidationAttributes(
            Type modelType,
            PropertyInfo property,
            string name)
        {
            return new Dictionary<string, string>
            {
                {DataValAttribute, "true"},
                {DataTypeDryAttribute, property.PropertyType.GetJavaScriptType()},
                {DataValDryAttribute, name }
            };
        }
    }
}