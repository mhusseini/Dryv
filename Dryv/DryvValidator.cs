using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Compilation;
using Dryv.Utils;

namespace Dryv
{
    public class DryvValidator
    {
        private static readonly ConstructorInfo ValidationResultCtor = typeof(DryvValidationError).GetConstructors().First();
        private static readonly MethodInfo AddResultMethod = typeof(ICollection<DryvValidationError>).GetMethod(nameof(ICollection<DryvValidationError>.Add));
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });
        private static readonly MethodInfo ValidateMethod = typeof(DryvValidator).GetMethod(nameof(Validate), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly ConcurrentDictionary<Type, ValidateAction> ValidateMethods = new ConcurrentDictionary<Type, ValidateAction>();
        private static readonly MethodInfo ValidatePropertyMethod = typeof(DryvValidator).GetMethod(nameof(ValidateProperty), BindingFlags.NonPublic | BindingFlags.Static);

        private delegate void ValidateAction(object currentModel,
            object rootModel,
            string path,
            IList<DryvValidationError> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache);

        public List<DryvValidationError> Validate(
            object model,
            Func<Type, object> services = null,
            IDictionary<object, object> cache = null)
        {
            var result = new List<DryvValidationError>();
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

            Validate(model, model, string.Empty, result, new HashSet<object>(), services, cache);

            return result;
        }

        internal static string ValidateProperty(
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

            return (from node in rootModelType.GetRulesForProperty(property, modelPath)
                    where node.Rule.IsEnabled(services) &&
                          node.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                    let model = treeInfo.FindModel(node.Rule.ModelPath, currentModel, cache)
                    let result = node.Rule.Validate(model, services)
                    where result.IsError()
                    select result.Message).FirstOrDefault();
        }

        private static ValidateAction CreateValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvValidationError>), "result");
            var parameterPath = Expression.Parameter(typeof(string), "path");

            var typedModel = Expression.Convert(parameterModel, type);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var navigationProperties = (from p in properties
                                        where (p.PropertyType.IsClass || p.PropertyType.IsInterface)
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

        private static void ValidateSingleItem(
            object model,
            object rootModel,
            string path,
            IList<DryvValidationError> result,
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

        private static void Validate(
            object input,
            object rootModel,
            string path,
            IList<DryvValidationError> result,
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
    }
}