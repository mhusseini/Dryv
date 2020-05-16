using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.RuleDetection;
using Dryv.Translation;
using Dryv.Validation;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public class DryvClientValidationLoader
    {
        private readonly IDryvClientValidationFunctionWriter clientValidationFunctionWriter;

        private readonly DryvRulesFinder rulesFinder = new DryvRulesFinder();

        private readonly DryvRuleTranslator translator;

        public DryvClientValidationLoader(IDryvClientValidationFunctionWriter clientValidationFunctionWriter, IOptions<DryvOptions> options, IServiceProvider serviceProvider)
        {
            this.clientValidationFunctionWriter = clientValidationFunctionWriter;
            this.translator = new DryvRuleTranslator(options.Value, serviceProvider.GetService);
        }

        public DryvClientValidationItem GetDryvClientValidation<TModel>(Expression<Func<TModel, object>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"The parameter '{nameof(propertyExpression)}' must be a property.");
            }

            return this.GetDryvClientValidation(typeof(TModel), property);
        }

        public DryvClientValidationItem GetDryvClientValidation(Type modelType, PropertyInfo property)
        {
            return this.GetClientPropertyValidation(modelType, string.Empty, property);
        }

        public IList<DryvClientValidationItem> GetDryvClientValidation<TModel>()
        {
            return this.GetDryvClientValidation(typeof(TModel));
        }

        public IList<DryvClientValidationItem> GetDryvClientValidation(Type modelType)
        {
            return this.CollectClientValidation(modelType, modelType, string.Empty, new Stack<string>());
        }

        internal IList<DryvClientValidationItem> CollectClientValidation(Type modelType, Type rootModelType, string modelPath, Stack<string> processedTypes)
        {
            if (modelType == typeof(Type) ||
                modelType.Namespace?.StartsWith("System.Reflection") == true ||
                modelType.FullName == null ||
                processedTypes.Contains(modelType.FullName))
            {
                return new DryvClientValidationItem[0];
            }

            var properties = modelType.GetProperties();

            var items = from property in properties
                        let item = this.GetClientPropertyValidation(modelType, modelPath, property)
                        where item != null
                        select item;

            processedTypes.Push(modelType.FullName);

            var childItems = from property in properties
                             let prefix = string.IsNullOrWhiteSpace(modelPath) ? string.Empty : $"{modelPath}."
                             let childPath = $"{prefix}{property.Name.ToCamelCase()}"
                             where !property.PropertyType.IsValueType && !property.PropertyType.HasElementType && property.PropertyType != typeof(string) && !processedTypes.Contains(childPath)
                             from item in this.CollectClientValidation(property.PropertyType, rootModelType, childPath, processedTypes)
                             select item;

            processedTypes.Pop();

            return items.Union(childItems).ToList();
        }

        private DryvClientValidationItem GetClientPropertyValidation(
                                    Type modelType,
            string modelPath,
            PropertyInfo property)
        {
            if (modelPath == null)
            {
                modelPath = string.Empty;
            }

            var rules = from rule in this.rulesFinder.GetRulesForProperty(modelType, property, modelPath)
                        where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                        select rule;

            var translatedRules = this.translator.Translate(rules, modelPath, modelType);
            var key = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";
            var validationFunction = translatedRules.Any() ? this.clientValidationFunctionWriter.GetValidationFunction(translatedRules) : null;

            return string.IsNullOrWhiteSpace(validationFunction) ? null : new DryvClientValidationItem
            {
                ValidationFunction = validationFunction,
                Key = key,
                ModelType = modelType,
                Property = property,
                ModelPath = modelPath,
            };
        }
    }
}