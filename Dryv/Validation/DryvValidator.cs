using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Compilation;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.Reflection;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Validation
{
    public class DryvValidator
    {
        private static readonly ConcurrentDictionary<Type, DryvServerValidationMethodCompiler.ValidateAsyncAction> ValidateAsyncMethods = new ConcurrentDictionary<Type, DryvServerValidationMethodCompiler.ValidateAsyncAction>();
        private static readonly ConcurrentDictionary<Type, DryvServerValidationMethodCompiler.ValidateAction> ValidateMethods = new ConcurrentDictionary<Type, DryvServerValidationMethodCompiler.ValidateAction>();

        public static List<DryvResult> Validate(object model, Func<Type, object> services = null)
        {
            return ValidateCore(model, services ?? (t => null), new Dictionary<object, object>()).Where(r => r.Message.Any(vr => !vr.IsSuccess())).ToList();
        }

        public static async Task<List<DryvResult>> ValidateAsync(
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
            IList<DryvResult> result,
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
            IList<DryvAsyncResult> result,
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

            return DryvReflectionRulesProvider.GetCompiledRulesForProperty(rootModelType, property, services, modelPath);
        }

        private static IEnumerable<KeyValuePair<object, DryvRuleDefinition>> GetModelsAndRules(object currentModel, object rootModel, PropertyInfo property, Func<Type, object> services, IDictionary<object, object> cache, bool syncAllowed, bool asyncAllowed)
        {
            var treeInfo = GetTreeInfo(currentModel, rootModel, cache);
            var ruleNodes = FindRulesForProperty(rootModel, property, services, treeInfo);

            return from node in ruleNodes
                   where node.Rule.EvaluationLocation.HasFlag(DryvRuleLocation.Server)
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

        private static IEnumerable<DryvResult> ValidateCore(object model, Func<Type, object> services, IDictionary<object, object> cache)
        {
            var result = new List<DryvResult>();
            if (model == null)
            {
                return result;
            }

            ValidatePath(model, model, String.Empty, result, new HashSet<object>(), services, cache);

            return result;
        }

        private static async Task<IList<DryvResult>> ValidateCoreAsync(object model, Func<Type, object> services, IDictionary<object, object> cache, bool asyncOnly)
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
            await ValidatePathAsync(model, model, string.Empty, asyncResults, new HashSet<object>(), services, cache, asyncOnly);
            result.AddRange(await Task.WhenAll(asyncResults.Select(r => r.Task)));

            return result;
        }

        private static void ValidateSingleItem(
                    object model,
            object rootModel,
            string path,
            IList<DryvResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache = null)
        {
            if (model == null || processed.Contains(model))
            {
                return;
            }

            processed.Add(model);

            ValidateMethods.GetOrAdd(model.GetType(), DryvServerValidationMethodCompiler.CreateValidateMethods)(
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
            IList<DryvAsyncResult> result,
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

            ValidateAsyncMethods.GetOrAdd(model.GetType(), DryvServerValidationMethodCompiler.CreateAsyncValidateMethods)(
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