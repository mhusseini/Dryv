using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.Reflection;
using Dryv.Validation;

namespace Dryv.Compilation
{
    internal class DryvServerValidationMethodCompiler
    {
        private static readonly MethodInfo AddAsyncResultMethod = typeof(ICollection<DryvAsyncResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<Task<DryvAsyncResult>>.Add));
        private static readonly MethodInfo AddResultMethod = typeof(ICollection<DryvResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<DryvResult>.Add));
        private static readonly ConstructorInfo AsyncValidationResultCtor = typeof(DryvAsyncResult).GetTypeInfo().DeclaredConstructors.First();
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetTypeInfo().DeclaredMethods.First(m => m.Name == nameof(string.Concat) && m.GetParameters().Select(p => p.ParameterType).ToList().ElementsEqual(typeof(string), typeof(string)));
        private static readonly MethodInfo ValidatePathAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePathAsync));
        private static readonly MethodInfo ValidatePathMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePath));
        private static readonly MethodInfo ValidatePropertyAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePropertyAsync));
        private static readonly MethodInfo ValidatePropertyMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidateProperty));
        private static readonly ConstructorInfo ValidationResultCtor = typeof(DryvResult).GetTypeInfo().DeclaredConstructors.First();

        internal delegate void ValidateAction(object currentModel,
            object rootModel,
            string path,
            IList<DryvResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache);

        internal delegate void ValidateAsyncAction(object currentModel,
            object rootModel,
            string path,
            IList<DryvAsyncResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            bool asyncOnly);

        public static ValidateAsyncAction CreateAsyncValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvAsyncResult>), "result");
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

        public static ValidateAction CreateValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvResult>), "result");
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
    }
}