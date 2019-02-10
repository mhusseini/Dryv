using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Compilation;
using Dryv.Reflection;
using Dryv.Utils;

namespace Dryv
{
    public class DryvValidator
    {
        private static readonly ConcurrentDictionary<Type, DryvServerValidationCompiler.ValidateAsyncAction> ValidateAsyncMethods = new ConcurrentDictionary<Type, DryvServerValidationCompiler.ValidateAsyncAction>();
        private static readonly ConcurrentDictionary<Type, DryvServerValidationCompiler.ValidateAction> ValidateMethods = new ConcurrentDictionary<Type, DryvServerValidationCompiler.ValidateAction>();

        public static List<DryvValidationResult> Validate(object model, Func<Type, object> services = null)
        {
            return ValidateCore(model, services ?? (t => null), new Dictionary<object, object>()).Where(r => r.Message.Any(vr => !vr.IsSuccess())).ToList();
        }

        public static async Task<List<DryvValidationResult>> ValidateAsync(
            object model,
            bool asyncOnly = false,
            Func<Type, object> services = null)
        {
            var result = await ValidateCoreAsync(model, services ?? (t => null), new Dictionary<object, object>(), asyncOnly);
            return result.Where(r => r.Message.Any(vr => !vr.IsSuccess())).ToList();
        }

        internal static void ValidatePath(
            object input,
            object rootModel,
            string path,
            IList<DryvValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache)
        {
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
                    ValidateSingleItem(model, rootModel, $"{path}[{i++}]{suffix}", result, processed, services, cache);
                }
            }
            else
            {
                ValidateSingleItem(input, rootModel, path, result, processed, services, cache);
            }
        }

        internal static async Task ValidatePathAsync(
            object input,
            object rootModel,
            string path,
            IList<DryvAsyncValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly)
        {
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
                    ValidateSingleItemAsync(model, rootModel, $"{path}[{i++}]{suffix}", result, processed, services, cache, asyncOnly);
                }
            }
            else
            {
                ValidateSingleItemAsync(input, rootModel, path, result, processed, services, cache, asyncOnly);
            }
        }

        internal static IReadOnlyCollection<DryvResultMessage> ValidateProperty(
                            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache = null)
        {
            var modelRules = GetModelsAndRules(currentModel, rootModel, property, services, cache, true, false);
            return (from item in modelRules
                    select RuleCompiler.Validate(item.Value, item.Key, services)).ToList();
        }

        internal static async Task<IReadOnlyCollection<DryvResultMessage>> ValidatePropertyAsync(
                                            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool syncAllowed)
        {
            var modelRules = GetModelsAndRules(currentModel, rootModel, property, services, cache, syncAllowed, true);
            return await Task.WhenAll(from modelRule in modelRules
                                      select RuleCompiler.ValidateAsync(modelRule.Value, modelRule.Key, services));
        }

        private static IEnumerable<DryvRuleNode> FindRulesForProperty(object rootModel, PropertyInfo property, Func<Type, object> services, ModelTreeInfo treeInfo)
        {
            var rootModelType = rootModel.GetType();
            var modelPath = treeInfo.ModelsByPath.Keys.OrderBy(k => k.Length).Last();

            return DryvReflectionRulesProvider.GetRulesForProperty(rootModelType, property, modelPath);
        }

        private static IEnumerable<KeyValuePair<object, DryvRuleDefinition>> GetModelsAndRules(object currentModel, object rootModel, PropertyInfo property, Func<Type, object> services, IDictionary<object, object> cache, bool syncAllowed, bool asyncAllowed)
        {
            var treeInfo = GetTreeInfo(currentModel, rootModel, cache);
            var ruleNodes = FindRulesForProperty(rootModel, property, services, treeInfo);

            return from node in ruleNodes
                   where RuleCompiler.IsEnabled(node.Rule, services) &&
                         node.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                   let isAsync = typeof(Task).IsAssignableFrom(node.Rule.ValidationExpression.ReturnType)
                   where (asyncAllowed || !isAsync) && (!asyncAllowed || !syncAllowed || isAsync)
                   let model = treeInfo.FindModel(node.Rule.ModelPath, currentModel, cache)
                   select new KeyValuePair<object, DryvRuleDefinition>(model, node.Rule);
        }

        private static ModelTreeInfo GetTreeInfo(object model, object root, IDictionary<object, object> cache)
        {
            if (cache == null)
            {
                cache = new Dictionary<object, object>();
            }

            return cache.GetOrAdd(model, m => ModelTreeInfoExtensions.GetTreeInfo(m, root));
        }

        private static IEnumerable<DryvValidationResult> ValidateCore(object model, Func<Type, object> services, IDictionary<object, object> cache)
        {
            var result = new List<DryvValidationResult>();
            if (model == null)
            {
                return result;
            }

            ValidatePath(model, model, String.Empty, result, new HashSet<object>(), services, cache);

            return result;
        }

        private static async Task<IList<DryvValidationResult>> ValidateCoreAsync(object model, Func<Type, object> services, IDictionary<object, object> cache, bool asyncOnly)
        {
            var result = new List<DryvValidationResult>();
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

            var asyncResults = new List<DryvAsyncValidationResult>();
            await ValidatePathAsync(model, model, String.Empty, asyncResults, new HashSet<object>(), services, cache, asyncOnly);
            await Task.WhenAll(asyncResults.Select(r => r.Task));

            result.AddRange(from r in asyncResults
                            select new DryvValidationResult(r.Model, r.Property, r.Path, r.Task.Result));

            return result;
        }

        private static void ValidateSingleItem(
                    object model,
            object rootModel,
            string path,
            IList<DryvValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache = null)
        {
            if (model == null || processed.Contains(model))
            {
                return;
            }

            processed.Add(model);

            ValidateMethods.GetOrAdd(model.GetType(), DryvServerValidationCompiler.CreateValidateMethods)(
                model,
                rootModel,
                path,
                result,
                processed,
                services,
                cache);
        }

        private static void ValidateSingleItemAsync(object model,
            object rootModel,
            string path,
            IList<DryvAsyncValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly)
        {
            if (model == null || processed.Contains(model))
            {
                return;
            }

            processed.Add(model);

            ValidateAsyncMethods.GetOrAdd(model.GetType(), DryvServerValidationCompiler.CreateAsyncValidateMethods)(
                model,
                rootModel,
                path,
                result,
                processed,
                services,
                cache,
                asyncOnly);
        }
    }
}