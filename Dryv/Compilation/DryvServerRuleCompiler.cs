using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.Reflection;
using Dryv.Validation;

namespace Dryv.Compilation
{
    internal class DryvServerRuleCompiler
    {
        private static readonly MethodInfo AddAsyncResultMethod = typeof(ICollection<DryvAsyncResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<DryvAsyncResult>.Add));
        private static readonly ConstructorInfo AsyncValidationResultCtor = typeof(DryvAsyncResult).GetTypeInfo().DeclaredConstructors.First();
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetTypeInfo().DeclaredMethods.First(m => m.Name == nameof(string.Concat) && m.GetParameters().Select(p => p.ParameterType).ToList().ElementsEqual(typeof(string), typeof(string)));
        private static readonly MethodInfo ValidatePathAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePathAsync));
        private static readonly MethodInfo ValidatePropertyAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePropertyAsync));

        internal delegate void ValidateAsyncAction(
            DryvValidator validator,
            object currentModel,
            object rootModel,
            string path,
            IList<DryvAsyncResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly);

        public ValidateAsyncAction CreateAsyncValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvAsyncResult>), "result");
            var parameterPath = Expression.Parameter(typeof(string), "path");
            var parameterAsyncOnly = Expression.Parameter(typeof(bool), "asyncOnly");
            var parameterValidator = Expression.Parameter(typeof(DryvValidator), "validator");

            var typedModel = Expression.Convert(parameterModel, type);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            var navigationProperties = (from p in properties
                                        where p.IsNavigationProperty()
                                        select p).ToList();

            var propertyValidationExpressions = from property in properties
                                                where !property.IsNavigationProperty() && property.GetCustomAttribute<DryvRulesAttribute>() != null
                                                select Expression.Call(parameterResult, AddAsyncResultMethod,
                                                    Expression.New(AsyncValidationResultCtor,
                                                        parameterModel,
                                                        Expression.Constant(property),
                                                        Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name)),
                                                        Expression.Call(
                                                            parameterValidator,
                                                            ValidatePropertyAsyncMethod,
                                                            typedModel,
                                                            parameterRootModel,
                                                            Expression.Constant(property),
                                                            parameterServices,
                                                            parameterCache,
                                                            parameterAsyncOnly)));

            var navigationExpressions = from property in navigationProperties
                                        select Expression.Call(
                                            parameterValidator,
                                            ValidatePathAsyncMethod,
                                            Expression.Property(typedModel, property),
                                            parameterRootModel,
                                            Expression.Constant(property),
                                            parameterModel,
                                            Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name + ".")),
                                            parameterResult,
                                            parameterProcessed,
                                            parameterServices,
                                            parameterCache);

            var lambda = Expression.Lambda<ValidateAsyncAction>(
                Expression.Block(propertyValidationExpressions.Union(navigationExpressions)),
                parameterValidator,
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
    }
}