using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.Reflection;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Validation
{
    public class DryvValidator
    {
        private static readonly ConcurrentDictionary<Type, DryvServerRuleCompiler.ValidateAsyncAction> ValidateAsyncMethods = new ConcurrentDictionary<Type, DryvServerRuleCompiler.ValidateAsyncAction>();

        private readonly DryvServerRuleCompiler ruleCompiler;
        private readonly DryvServerRuleEvaluator ruleEvaluator;
        private readonly DryvRulesFinder ruleFinder;

        public DryvValidator()
        {
            this.ruleFinder = new DryvRulesFinder();
            this.ruleEvaluator = new DryvServerRuleEvaluator();
            this.ruleCompiler = new DryvServerRuleCompiler();
        }

        public async Task<List<DryvResult>> Validate(
            object model,
            Func<Type, object> services = null)
        {
            var result = await this.ValidateCoreAsync(model, services ?? (t => null), new Dictionary<object, object>());
            return result.Where(r => r.Message.Any(vr => !vr.IsSuccess())).ToList();
        }

        internal async Task ValidatePathAsync(
            object input,
            object rootModel,
            PropertyInfo property,
            object parentModel,
            string path,
            IList<DryvAsyncResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache)
        {
            if (parentModel != null && property != null)
            {
                var disablingRules = this.GetModelsAndRules(parentModel, rootModel, property, services, cache, true, true, RuleType.Disabling);
                var disabled = await Task.WhenAll(from modelRule in disablingRules
                                                  select this.ruleEvaluator.IsDisabledAsync(modelRule.Value, modelRule.Key, services));
                if (disabled.Any(v => v))
                {
                    return;
                }
            }

            if (input is IEnumerable items)
            {
                string suffix;
                if (path.EndsWith("."))
                {
                    path = path.TrimEnd('.');
                    suffix = ".";
                }
                else
                {
                    suffix = string.Empty;
                }

                var i = 0;
                foreach (var model in items.OfType<object>())
                {
                    this.ValidateSingleItemAsync(model, rootModel, $"{path}[{i++}]{suffix}", result, processed, services, cache);
                }
            }
            else
            {
                this.ValidateSingleItemAsync(input, rootModel, path, result, processed, services, cache);
            }
        }

        internal IEnumerable<DryvResultMessage> ValidateProperty(
            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            DryvOptions options)
        {
            var disablingRules = this.GetModelsAndRules(currentModel, rootModel, property, services, cache, true, false, RuleType.Disabling);
            if (disablingRules.Any(rule => this.ruleEvaluator.IsDisabled(rule.Value, rule.Key, services)))
            {
                yield break;
            }

            var rules = this.GetModelsAndRules(currentModel, rootModel, property, services, cache, true, false, RuleType.Default);
            foreach (var rule in rules)
            {
                var result = this.ruleEvaluator.Validate(rule.Value, rule.Key, services);
                if (result.IsSuccess())
                {
                    continue;
                }

                yield return result;

                if (options.BreakOnFirstValidationError)
                {
                    yield break;
                }
            }
        }

        internal async Task<IReadOnlyCollection<DryvResultMessage>> ValidatePropertyAsync(
                                            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly)
        {
            var modelRules = this.GetModelsAndRules(currentModel, rootModel, property, services, cache, !asyncOnly, true, RuleType.Default);
            return await Task.WhenAll(from modelRule in modelRules
                                      select this.ruleEvaluator.ValidateAsync(modelRule.Value, modelRule.Key, services));
        }

        private static ModelTreeInfo GetTreeInfo(object model, object root, IDictionary<object, object> cache)
        {
            if (cache == null)
            {
                cache = new Dictionary<object, object>();
            }

            return cache.GetOrAdd(model, m => m.GetTreeInfo(root));
        }

        private IEnumerable<DryvRuleTreeNode> FindRulesForProperty(Type rootModelType, PropertyInfo property, Func<Type, object> services, ModelTreeInfo treeInfo, RuleType ruleType)
        {
            var modelPath = treeInfo.ModelsByPath.Keys.OrderBy(k => k.Length).Last();

            return from rule in this.ruleFinder.GetRulesForProperty(rootModelType, property, ruleType, modelPath)
                   where this.ruleEvaluator.IsEnabled(rule.Rule, services)
                   select rule;
        }

        private IEnumerable<KeyValuePair<object, DryvCompiledRule>> GetModelsAndRules(object currentModel, object rootModel, PropertyInfo property, Func<Type, object> services, IDictionary<object, object> cache, bool syncAllowed, bool asyncAllowed, RuleType ruleType)
        {
            var treeInfo = GetTreeInfo(currentModel, rootModel, cache);
            var ruleNodes = this.FindRulesForProperty(rootModel.GetType(), property, services, treeInfo, ruleType);

            return (from node in ruleNodes
                    where node.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Server)
                    let isAsync = typeof(Task).IsAssignableFrom(node.Rule.ValidationExpression.ReturnType)
                    where (asyncAllowed && isAsync) || (syncAllowed && !isAsync)
                    let model = treeInfo.FindModel(node.Rule.ModelPath, currentModel, cache)
                    select new KeyValuePair<object, DryvCompiledRule>(model, node.Rule)).ToList();
        }

        private async Task<IList<DryvResult>> ValidateCoreAsync(object model, Func<Type, object> services, IDictionary<object, object> cache)
        {
            var result = new List<DryvResult>();
            if (model == null)
            {
                return result;
            }

            if (cache == null)
            {
                cache = new Dictionary<object, object>();
            }

            if (services == null)
            {
                services = t => null;
            }

            var asyncResults = new List<DryvAsyncResult>();
            await this.ValidatePathAsync(model, model, null, null, string.Empty, asyncResults, new HashSet<object>(), services, cache);
            result.AddRange(await Task.WhenAll(asyncResults.Select(r => r.Task)));

            return result;
        }

        private void ValidateSingleItemAsync(object model,
            object rootModel,
            string path,
            IList<DryvAsyncResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache)
        {
            if (model == null || processed.Contains(model))
            {
                return;
            }

            processed.Add(model);

            ValidateAsyncMethods.GetOrAdd(model.GetType(), this.ruleCompiler.CreateAsyncValidateMethods)(
                this,
                model,
                rootModel,
                path,
                result,
                processed,
                services,
                cache,
                false);
        }
    }
}