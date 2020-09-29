using System;
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
        private static readonly ConcurrentDictionary<Type, GroupedValidation> Cache = new ConcurrentDictionary<Type, GroupedValidation>();
        private readonly DryvOptions options;
        private readonly DryvRuleFinder ruleFinder;

        public DryvValidator(DryvRuleFinder ruleFinder, DryvOptions options)
        {
            this.ruleFinder = ruleFinder;
            this.options = options;
        }

        public async Task<IDictionary<string, DryvValidationResult>> Validate(object model, Func<Type, object> serviceProvider)
        {
            if (model == null)
            {
                return new Dictionary<string, DryvValidationResult>();
            }

            var modelType = model.GetType();
            var validation = Cache.GetOrAdd(modelType, this.GroupValidation);

            var rules = validation.ValidationRules.SelectMany(i => i.Value).Union(validation.DisablingRules.SelectMany(i => i.Value));
            var parameters = await DryvParametersHelper.GetDryvParameters(rules, serviceProvider);
            var taskResults = await Task.WhenAll(from kvp in validation.ValidationRules
                                                 where !this.IsSubtreeDisabled(model, kvp.Key, validation.DisablingRules, serviceProvider, parameters)
                                                 select this.GetFirstValidationError(model, serviceProvider, kvp, parameters));

            return (from result in taskResults
                    where result.HasValue
                    select result.Value)
                .ToDictionary(i => i.Key, i => i.Value);
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

        private async Task<KeyValuePair<string, DryvValidationResult>?> GetFirstValidationError(object model, Func<Type, object> serviceProvider, KeyValuePair<string, List<DryvCompiledRule>> kvp, Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            foreach (var rule in kvp.Value)
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
                        return null;
                    }

                    result.Group = rule.Group;

                    return new KeyValuePair<string, DryvValidationResult>(kvp.Key, result);
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
            return new GroupedValidation
            {
                ValidationRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Validation)
                    .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Server))
                    .GroupBy(r => r.ModelPath)
                    .ToDictionary(g => g.Key, g => g.ToList()),

                DisablingRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Disabling)
                    .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Server))
                    .GroupBy(r => r.ModelPath)
                    .ToDictionary(g => g.Key, g => g.ToList())
            };
        }

        private bool IsSubtreeDisabled(object model, string modelPath, IReadOnlyDictionary<string, List<DryvCompiledRule>> disablingRules, Func<Type, object> serviceProvider, Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            return disablingRules.TryGetValue(modelPath, out var disablers) && disablers.Any(rule =>
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
            });
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