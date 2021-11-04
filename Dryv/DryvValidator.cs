using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Validation;

namespace Dryv
{
    public class DryvValidator
    {
        private static readonly ConcurrentDictionary<Type, GroupedValidation> Cache = new();
        private readonly DryvOptions options;
        private readonly DryvRuleFinder ruleFinder;

        public DryvValidator(DryvRuleFinder ruleFinder, DryvOptions options)
        {
            this.ruleFinder = ruleFinder;
            this.options = options;
        }

        public async Task<IDictionary<string, DryvValidationResult>> Validate(object model, Func<Type, object> serviceProvider, IReadOnlyDictionary<string, object> parameters = null)
        {
            var errors = new Dictionary<string, DryvValidationResult>();
            var processed = new HashSet<object>();

            var modelType = model.GetType();
            var validation = Cache.GetOrAdd(modelType, this.GroupValidation);

            var rules = validation.ValidationRules.SelectMany(i => i.Value).Union(validation.DisablingRules.SelectMany(i => i.Value));
            var ruleParameters = await DryvParametersHelper.GetDryvParameters(rules, serviceProvider, parameters);
            var currentRules = validation.ValidationRules;

            await Validate(model, string.Empty, serviceProvider, processed, currentRules, validation, ruleParameters, errors);

            return errors;
        }

        private async Task Validate(object model, string path, Func<Type, object> serviceProvider, HashSet<object> processed, Dictionary<string, List<DryvCompiledRule>> currentRules, GroupedValidation validation, Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> ruleParameters, Dictionary<string, DryvValidationResult> errors)
        {
            if (model == null || processed.Contains(model))
            {
                return;
            }

            processed.Add(model);
            var modelType = model.GetType();

            var taskResults = await Task.WhenAll(
                from kvp in currentRules
                let rs = kvp.Value.Where(rule => rule.ModelType == modelType).ToList()
                where rs.Any()
                where !this.IsSubtreeDisabled(model, kvp.Key, validation.DisablingRules, serviceProvider, ruleParameters)
                select this.GetFirstValidationError(model, kvp.Key, rs, serviceProvider, ruleParameters));

            errors.AddRange(from result in taskResults
                            where result.HasValue
                            select new KeyValuePair<string, DryvValidationResult>(path + result.Value.Key, result.Value.Value));

            foreach (var prop in from prop in modelType.GetProperties()
                                 where prop.IsNavigationList()
                                 select prop)
            {
                if (prop.GetValue(model) is not IEnumerable l)
                {
                    continue;
                }

                var index = 0;

                foreach (var child in l.Cast<object>())
                {
                    await Validate(child, $"{path}{prop.Name.ToCamelCase()}[{index++}].", serviceProvider, processed, currentRules, validation, ruleParameters, errors);
                }
            }
        }

        private static async Task<DryvValidationResult> TryGetValidationResult(object model, DryvCompiledRule rule, object[] services)
        {
            switch (rule.CompiledValidationExpression(model, services))
            {
                case DryvValidationResult r when r.IsSuccess():
                    return null;

                case DryvValidationResult r:
                    return r;

                case Task<DryvValidationResult> task:
                    var result = await task;
                    return result.IsSuccess() ? null : result;

                default:
                    return null;
            }
        }

        private async Task<KeyValuePair<string, DryvValidationResult>?> GetFirstValidationError(object model, string key, List<DryvCompiledRule> rules, Func<Type, object> serviceProvider, Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            foreach (var rule in rules)
            {
                try
                {
                    var services = serviceProvider.GetServices(rule, parameters);
                    if (!true.Equals(await TaskValueHelper.GetPossiblyAsyncValue(rule.CompiledEnablingExpression(services))))
                    {
                        continue;
                    }

                    var result = await TryGetValidationResult(model, rule, services);
                    if (result == null)
                    {
                        continue;
                    }

                    result.Group = rule.Group;

                    return new KeyValuePair<string, DryvValidationResult>(key, result);
                }
                catch (Exception ex)
                {
                    throw this.ThrowValidationException(model, rule, ex, RuleType.Validation);
                }
            }

            return null;
        }

        private GroupedValidation GroupValidation(Type type)
        {
            return new()
            {
                ValidationRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Validation)
                    .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Server))
                    .GroupBy(r => r.ModelPath) // RegexGroup.Replace(r.ModelPath, string.Empty))
                    .ToDictionary(g => g.Key, g => g.ToList()),

                DisablingRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Disabling)
                    .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Server))
                    .GroupBy(r => r.ModelPath) //RegexGroup.Replace(r.ModelPath, string.Empty))
                    .ToDictionary(g => g.Key, g => g.ToList())
            };
        }

        private bool IsSubtreeDisabled(object model, string modelPath, IReadOnlyDictionary<string, List<DryvCompiledRule>> disablingRules, Func<Type, object> serviceProvider, Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            return FindDisablingRules(modelPath, disablingRules)?.Any(rule =>
            {
                try
                {
                    var services = serviceProvider.GetServices(rule, parameters);
                    var o = rule.CompiledValidationExpression(model, services);

                    return !(o is bool) || (bool)o;
                }
                catch (Exception ex)
                {
                    throw this.ThrowValidationException(model, rule, ex, RuleType.Disabling);
                }
            }) == true;
        }

        private List<DryvCompiledRule> FindDisablingRules(string modelPath, IReadOnlyDictionary<string, List<DryvCompiledRule>> disablingRules)
        {
            var parts = modelPath.Split('.');

            foreach (var part in Enumerable
                .Range(1, parts.Length)
                .Select(i => string.Join(".", parts.Take(i))))
            {
                if (disablingRules.TryGetValue(part, out var result))
                {
                    return result;
                }
            }

            return null;
        }

        private Exception ThrowValidationException(object model, DryvCompiledRule rule, Exception innerException, RuleType ruleType)
        {
            var json = this.options.JsonConversion(model);
            var expressionText = rule.ValidationExpression.ToString();
            var sb = new StringBuilder($"An error occurred executing the {ruleType.ToString().ToLowerInvariant()} expression '{expressionText}' for property '{rule.Property.DeclaringType.Name}.{rule.Property.Name}'. See the inner exception for details.");
            if (this.options.IncludeModelDataInExceptions)
            {
                sb.AppendLine("The model being validated is:");
                sb.Append(json);
            }

            return new DryvValidationException(sb.ToString(), innerException);
        }

        private class GroupedValidation
        {
            public Dictionary<string, List<DryvCompiledRule>> DisablingRules { get; set; }

            public Dictionary<string, List<DryvCompiledRule>> ValidationRules { get; set; }
        }
    }
}