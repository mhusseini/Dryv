using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Rework.RuleDetection;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Rework
{
    public class DryvValidator
    {
        private static readonly ConcurrentDictionary<Type, GroupedValidation> Cache = new ConcurrentDictionary<Type, GroupedValidation>();
        private readonly DryvRuleFinder ruleFinder;

        public DryvValidator(DryvRuleFinder ruleFinder)
        {
            this.ruleFinder = ruleFinder;
        }

        public async Task<IDictionary<string, DryvValidationResult>> Validate(object model, Func<Type, object> serviceProvider)
        {
            if (model == null)
            {
                return new Dictionary<string, DryvValidationResult>();
            }

            var modelType = model.GetType();
            var validation = Cache.GetOrAdd(modelType, this.GroupValidation);
            var taskResults = await Task.WhenAll(from kvp in validation.ValidationRules
                                                 where !IsSubtreeDisabled(model, kvp.Key, validation.DisablingRules, serviceProvider)
                                                 select GetFirstValidationError(model, serviceProvider, kvp));

            return (from result in taskResults
                    where result.HasValue
                    select result.Value)
                .ToDictionary(i => i.Key, i => i.Value);
        }

        private static async Task<KeyValuePair<string, DryvValidationResult>?> GetFirstValidationError(object model, Func<Type, object> serviceProvider, KeyValuePair<string, List<DryvCompiledRule>> kvp)
        {
            foreach (var rule in kvp.Value)
            {
                var services = rule.PreevaluationOptionTypes.Select(serviceProvider).ToArray();
                if (!rule.CompiledEnablingExpression(services))
                {
                    continue;
                }

                var result = await TryGetValidationResult(model, rule, services);
                if (result == null)
                {
                    return null;
                }

                result.GroupName = rule.GroupName;

                return new KeyValuePair<string, DryvValidationResult>(kvp.Key, result);
            }

            return null;
        }

        private static bool IsSubtreeDisabled(object model, string modelPath, IReadOnlyDictionary<string, List<DryvCompiledRule>> disablingRules, Func<Type, object> serviceProvider)
        {
            return disablingRules.TryGetValue(modelPath, out var disablers) && disablers.Any(d =>
            {
                var services = d.PreevaluationOptionTypes.Select(serviceProvider).ToArray();
                var o = d.CompiledValidationExpression(model, services);

                return !(o is bool) || (bool)o;
            });
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

        private GroupedValidation GroupValidation(Type type)
        {
            return new GroupedValidation
            {
                ValidationRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Default)
                    .GroupBy(r => r.ModelPath)
                    .ToDictionary(g => g.Key, g => g.ToList()),

                DisablingRules = this.ruleFinder.FindValidationRulesInTree(type, RuleType.Disabling)
                    .GroupBy(r => r.ModelPath)
                    .ToDictionary(g => g.Key, g => g.ToList())
            };
        }

        private class GroupedValidation
        {
            public Dictionary<string, List<DryvCompiledRule>> DisablingRules { get; set; }
            public Dictionary<string, List<DryvCompiledRule>> ValidationRules { get; set; }
        }
    }
}