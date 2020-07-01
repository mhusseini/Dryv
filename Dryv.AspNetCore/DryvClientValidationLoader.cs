using System;
using System.Collections.Generic;
using System.IO;
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

        public IDictionary<string, Action<TextWriter>> GetDryvClientDisablingFunctions<TModel>() => GetDryvClientDisablingFunctions(typeof(TModel));

        public IDictionary<string, Action<TextWriter>> GetDryvClientDisablingFunctions(Type modelType)
        {
            return (from val in this.GetDryvClientValidation(modelType)
                    where val.DisablingFunction != null
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.DisablingFunction);
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

        public IDictionary<string, Action<TextWriter>> GetDryvClientValidationFunctions<TModel>() => this.GetDryvClientValidationFunctions(typeof(TModel));

        public IDictionary<string, Action<TextWriter>> GetDryvClientValidationFunctions(Type modelType)
        {
            return (from val in this.GetDryvClientValidation(modelType)
                    where val.ValidationFunction != null
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.ValidationFunction);
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

            var rules = from rule in this.rulesFinder.GetRulesForProperty(modelType, property, RuleType.Default, modelPath)
                        where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                        select rule;

            var disablingRules = from rule in this.rulesFinder.GetRulesForProperty(modelType, property, RuleType.Disabling, modelPath)
                                 where rule.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Client)
                                 select rule;

            var validationFunction = this.TranslateRules(modelType, modelPath, rules);
            var disablingFunction = this.TranslateRules(modelType, modelPath, disablingRules);

            var key = $"v{Math.Abs((modelType.FullName + property.Name + modelPath).GetHashCode())}";

            return validationFunction == null && disablingFunction == null ? null : new DryvClientValidationItem
            {
                ValidationFunction = validationFunction,
                DisablingFunction = disablingFunction,
                Key = key,
                ModelType = modelType,
                Property = property,
                ModelPath = modelPath,
            };
        }

        private Action<TextWriter> TranslateRules(Type modelType, string modelPath, IEnumerable<DryvRuleTreeNode> rules)
        {
            var translatedRules = this.translator.Translate(rules, modelPath, modelType);
            var validationFunction = translatedRules.Any() ? this.clientValidationFunctionWriter.GetValidationFunction(translatedRules) : null;
            return validationFunction;
        }
    }
}