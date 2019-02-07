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
        private static readonly MethodInfo ValidateAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidateCoreAsync));
        private static readonly ConcurrentDictionary<Type, ValidateAsyncAction> ValidateAsyncMethods = new ConcurrentDictionary<Type, ValidateAsyncAction>();
        private static readonly MethodInfo ValidateMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(ValidateCore));
        private static readonly ConcurrentDictionary<Type, ValidateAction> ValidateMethods = new ConcurrentDictionary<Type, ValidateAction>();
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

        public List<DryvValidationResult> Validate(object model, Func<Type, object> services = null)
        {
            return this.ValidateCore(model, services, new Dictionary<object, object>());
        }

        public Task<List<DryvValidationResult>> ValidateAsync(
            object model,
            bool asyncOnly = false,
            Func<Type, object> services = null)
        {
            return this.ValidateCoreAsync(model, asyncOnly, services, new Dictionary<object, object>());
        }

        internal static IReadOnlyCollection<DryvResult> ValidateProperty(
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
            var returnLabel = Expression.Label();
            var propertyValidationExpressions = from property in properties
                                                let task = Expression.Parameter(typeof(Task<IReadOnlyCollection<DryvResult>>), "task")
                                                select Expression.Block(
                                                    new[] { task },
                                                    Expression.Assign(
                                                        task,
                                                        Expression.Call(ValidatePropertyAsyncMethod,
                                                            typedModel,
                                                            parameterRootModel,
                                                            Expression.Constant(property),
                                                            parameterServices,
                                                            parameterCache,
                                                            parameterAsyncOnly)),
                                                    Expression.Call(parameterResult, AddAsyncResultMethod,
                                                        Expression.New(AsyncValidationResultCtor,
                                                            parameterModel,
                                                            Expression.Constant(property),
                                                            Expression.Call(StringConcatMethod,
                                                                parameterPath,
                                                                Expression.Constant(property.Name)),
                                                            task)),
                                                    Expression.Return(returnLabel, task),
                                                    Expression.Label(returnLabel)
                                                );
            var navigationExpressions = from property in navigationProperties
                                        let childVariable = Expression.Parameter(property.PropertyType, "child")
                                        select Expression.Block(
                                            new[] { childVariable },
                                            Expression.Assign(
                                                childVariable,
                                                Expression.Property(typedModel, property)),
                                            Expression.Return(returnLabel,
                                                Expression.Call(ValidateAsyncMethod,
                                                    childVariable,
                                                    parameterRootModel,
                                                    Expression.Call(StringConcatMethod,
                                                        parameterPath,
                                                        Expression.Constant(property.Name + ".")),
                                                    parameterResult,
                                                    parameterProcessed,
                                                    parameterServices,
                                                    parameterCache,
                                                    parameterAsyncOnly)),
                                            Expression.Label(returnLabel)
                                        );
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
                                                let errorVariable = Expression.Parameter(typeof(string), "error")
                                                select Expression.Block(
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
                                                                Expression.Call(StringConcatMethod,
                                                                    parameterPath,
                                                                    Expression.Constant(property.Name)),
                                                                errorVariable)))
                                                );
            var navigationExpressions = from property in navigationProperties
                                        let childVariable = Expression.Parameter(property.PropertyType, "child")
                                        select Expression.Block(
                                            new[] { childVariable },
                                            Expression.Assign(
                                                childVariable,
                                                Expression.Property(typedModel, property)),
                                                Expression.Call(ValidateMethod,
                                                    childVariable,
                                                    parameterRootModel,
                                                    Expression.Call(StringConcatMethod,
                                                        parameterPath,
                                                        Expression.Constant(property.Name + ".")),
                                                    parameterResult,
                                                    parameterProcessed,
                                                    parameterServices,
                                                    parameterCache)
                                            );
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

        private static void ValidatePath(
            object input,
            object rootModel,
            string path,
            IList<DryvValidationResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache = null)
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

        private static async Task<IReadOnlyCollection<DryvResult>> ValidatePropertyAsync(
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
                                      where !asyncOnly || typeof(Task).IsAssignableFrom(node.Rule.ValidationExpression.ReturnType)
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

        private static async Task ValidateSingleItemAsync(object model,
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

        private List<DryvValidationResult> ValidateCore(
                                                                                    object model,
            Func<Type, object> services = null,
            IDictionary<object, object> cache = null)
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

            ValidatePath(model, model, string.Empty, result, new HashSet<object>(), services, cache);

            return result;
        }

        private async Task<List<DryvValidationResult>> ValidateCoreAsync(
            object model,
            bool asyncOnly = false,
            Func<Type, object> services = null,
            IDictionary<object, object> cache = null)
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
                            from dr in r.Task.Result
                            select new DryvValidationResult(r.Model, r.Property, r.Path, dr));

            return result;
        }
    }
}