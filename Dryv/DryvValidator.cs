using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Compilation;
using Dryv.Reflection;
using Dryv.Utils;

namespace Dryv
{
    public class DryvValidator
    {
        private static readonly MethodInfo AddAsyncResultMethod = typeof(ICollection<DryvAsyncValidationResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<Task<DryvAsyncValidationResult>>.Add));
        private static readonly MethodInfo AddResultMethod = typeof(ICollection<DryvValidationResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<DryvValidationResult>.Add));
        private static readonly ConstructorInfo AsyncValidationResultCtor = typeof(DryvAsyncValidationResult).GetTypeInfo().DeclaredConstructors.First();
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetTypeInfo().DeclaredMethods.First(m => m.Name == nameof(string.Concat) && m.GetParameters().Select(p => p.ParameterType).ToList().ElementsEqual(typeof(string), typeof(string)));
        private static readonly ConcurrentDictionary<Type, ValidateAsyncAction> ValidateAsyncMethods = new ConcurrentDictionary<Type, ValidateAsyncAction>();
        private static readonly ConcurrentDictionary<Type, ValidateAction> ValidateMethods = new ConcurrentDictionary<Type, ValidateAction>();
        private static readonly MethodInfo ValidatePathAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidatePathAsync));
        private static readonly MethodInfo ValidatePathMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidatePath));
        private static readonly MethodInfo ValidatePropertyAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidatePropertyAsync));
        private static readonly MethodInfo ValidatePropertyMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidateProperty));
        private static readonly ConstructorInfo ValidationResultCtor = typeof(DryvValidationResult).GetTypeInfo().DeclaredConstructors.First();

        private delegate void ValidateAction(object currentModel,
            object rootModel,
            string path,
            IList<DryvValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache);

        private delegate void ValidateAsyncAction(object currentModel,
            object rootModel,
            string path,
            IList<DryvAsyncValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly);

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

        internal static IReadOnlyCollection<DryvResultMessage> ValidateProperty(
            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache = null)
        {
            if (cache == null)
            {
                cache = new Dictionary<object, object>();
            }

            var treeInfo = currentModel.GetTreeInfo(rootModel, cache);
            var rootModelType = rootModel.GetType();
            var modelPath = treeInfo.ModelsByPath.Keys.OrderBy(k => k.Length).Last();

            return (from node in RulesFinder.GetRulesForProperty(rootModelType, property, modelPath)
                    where RuleCompiler.IsEnabled(node.Rule, services) &&
                          node.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                    let model = treeInfo.FindModel(node.Rule.ModelPath, currentModel, cache)
                    let result = RuleCompiler.Validate(node.Rule, model, services)
                    select result).ToList();
        }

        private static ValidateAsyncAction CreateAsyncValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvAsyncValidationResult>), "result");
            var parameterPath = Expression.Parameter(typeof(string), "path");
            var parameterAsyncOnly = Expression.Parameter(typeof(bool), "asyncOnly");

            var typedModel = Expression.Convert(parameterModel, type);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var navigationProperties = (from p in properties
                                        where (p.PropertyType.IsClass() || p.PropertyType.IsInterface())
                                              && p.PropertyType != typeof(string)
                                        select p).ToList();

            var propertyValidationExpressions = from property in properties
                                                select Expression.Call(parameterResult, AddAsyncResultMethod,
                                                        Expression.New(AsyncValidationResultCtor,
                                                            parameterModel,
                                                            Expression.Constant(property),
                                                            Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name)),
                                                            Expression.Call(ValidatePropertyAsyncMethod,
                                                                typedModel,
                                                                parameterRootModel,
                                                                Expression.Constant(property),
                                                                parameterServices,
                                                                parameterCache,
                                                                parameterAsyncOnly)));

            var navigationExpressions = from property in navigationProperties
                                        select Expression.Call(ValidatePathAsyncMethod,
                                            Expression.Property(typedModel, property),
                                            parameterRootModel,
                                            Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name + ".")),
                                            parameterResult,
                                            parameterProcessed,
                                            parameterServices,
                                            parameterCache,
                                            parameterAsyncOnly);

            var lambda = Expression.Lambda<ValidateAsyncAction>(
                Expression.Block(propertyValidationExpressions.Union(navigationExpressions)),
                parameterModel,
                parameterRootModel,
                parameterPath,
                parameterResult,
                parameterProcessed,
                parameterServices,
                parameterCache,
                parameterAsyncOnly);

            return lambda.Compile();
        }

        private static ValidateAction CreateValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvValidationResult>), "result");
            var parameterPath = Expression.Parameter(typeof(string), "path");
            var typedModel = Expression.Convert(parameterModel, type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            var navigationProperties = (from p in properties
                                        where (p.PropertyType.IsClass() || p.PropertyType.IsInterface())
                                              && p.PropertyType != typeof(string)
                                        select p).ToList();

            var propertyValidationExpressions = from property in properties
                                                let errorVariable = Expression.Parameter(typeof(IReadOnlyCollection<DryvResultMessage>), "error")
                                                select (Expression)Expression.Block(
                                                    new[] { errorVariable },
                                                    Expression.Assign(
                                                        errorVariable,
                                                        Expression.Call(ValidatePropertyMethod,
                                                            typedModel,
                                                            parameterRootModel,
                                                            Expression.Constant(property),
                                                            parameterServices,
                                                            parameterCache)),
                                                    Expression.IfThen(
                                                        Expression.Not(Expression.Equal(errorVariable, Expression.Constant(null))),
                                                        Expression.Call(parameterResult, AddResultMethod,
                                                            Expression.New(ValidationResultCtor,
                                                                parameterModel,
                                                                Expression.Constant(property),
                                                                Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name)),
                                                                errorVariable))));

            var navigationExpressions = from property in navigationProperties
                                        let childVariable = Expression.Parameter(property.PropertyType, "child")
                                        select Expression.Call(ValidatePathMethod,
                                            Expression.Property(typedModel, property),
                                            parameterRootModel,
                                            Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name + ".")),
                                            parameterResult,
                                            parameterProcessed,
                                            parameterServices,
                                            parameterCache);

            var lambda = Expression.Lambda<ValidateAction>(
                Expression.Block(propertyValidationExpressions.Union(navigationExpressions)),
                parameterModel,
                parameterRootModel,
                parameterPath,
                parameterResult,
                parameterProcessed,
                parameterServices,
                parameterCache);

            return lambda.Compile();
        }

        private static IEnumerable<DryvValidationResult> ValidateCore(object model, Func<Type, object> services, IDictionary<object, object> cache)
        {
            var result = new List<DryvValidationResult>();
            if (model == null)
            {
                return result;
            }

            ValidatePath(model, model, string.Empty, result, new HashSet<object>(), services, cache);

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
            await ValidatePathAsync(model, model, string.Empty, asyncResults, new HashSet<object>(), services, cache, asyncOnly);
            await Task.WhenAll(asyncResults.Select(r => r.Task));

            result.AddRange(from r in asyncResults
                            select new DryvValidationResult(r.Model, r.Property, r.Path, r.Task.Result));

            return result;
        }

        private static void ValidatePath(
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

        private static async Task ValidatePathAsync(
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

        private static async Task<IReadOnlyCollection<DryvResultMessage>> ValidatePropertyAsync(
                                            object currentModel,
            object rootModel,
            PropertyInfo property,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly)
        {
            if (cache == null)
            {
                cache = new Dictionary<object, object>();
            }

            var treeInfo = currentModel.GetTreeInfo(rootModel, cache);
            var rootModelType = rootModel.GetType();
            var modelPath = treeInfo.ModelsByPath.Keys.OrderBy(k => k.Length).Last();

            return await Task.WhenAll(from node in RulesFinder.GetRulesForProperty(rootModelType, property, modelPath)
                                      where RuleCompiler.IsEnabled(node.Rule, services) &&
                                            node.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                                      where typeof(Task).IsAssignableFrom(node.Rule.ValidationExpression.ReturnType) || !asyncOnly
                                      let model = treeInfo.FindModel(node.Rule.ModelPath, currentModel, cache)
                                      select RuleCompiler.ValidateAsync(node.Rule, model, services));
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

            ValidateMethods.GetOrAdd(model.GetType(), CreateValidateMethods)(
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

            ValidateAsyncMethods.GetOrAdd(model.GetType(), CreateAsyncValidateMethods)(
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