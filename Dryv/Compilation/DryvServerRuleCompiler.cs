using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Internal;
using Dryv.Reflection;
using Dryv.Validation;

namespace Dryv.Compilation
{
    internal class DryvServerRuleCompiler
    {
        private static readonly PropertyInfo BreakOnFirstValidationErrorProperty = typeof(DryvOptions).GetTypeInfo().GetDeclaredProperty(nameof(DryvOptions.BreakOnFirstValidationError));
        private static readonly MethodInfo AddAsyncResultMethod = typeof(ICollection<DryvAsyncResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<DryvAsyncResult>.Add));
        private static readonly PropertyInfo CountProperty = typeof(ICollection).GetTypeInfo().GetDeclaredProperty(nameof(ICollection.Count));
        private static readonly MethodInfo AddResultMethod = typeof(ICollection<DryvResult>).GetTypeInfo().GetDeclaredMethod(nameof(ICollection<DryvResult>.Add));
        private static readonly ConstructorInfo AsyncValidationResultCtor = typeof(DryvAsyncResult).GetTypeInfo().DeclaredConstructors.First();
        private static readonly MethodInfo StringConcatMethod = typeof(string).GetTypeInfo().DeclaredMethods.First(m => m.Name == nameof(string.Concat) && m.GetParameters().Select(p => p.ParameterType).ToList().ElementsEqual(typeof(string), typeof(string)));
        private static readonly MethodInfo ValidatePathAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePathAsync));
        private static readonly MethodInfo ValidatePathMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePath));
        private static readonly MethodInfo ValidatePropertyAsyncMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidatePropertyAsync));
        private static readonly MethodInfo ValidatePropertyMethod = typeof(DryvValidator).GetTypeInfo().GetDeclaredMethod(nameof(DryvValidator.ValidateProperty));
        private static readonly ConstructorInfo ValidationResultCtor = typeof(DryvResult).GetTypeInfo().DeclaredConstructors.First();

        internal delegate void ValidateAction(
            DryvValidator validator,
            object currentModel,
                    object rootModel,
            string path,
            IList<DryvResult> result,
            ICollection<object> processed,
            Func<Type, object> services,
            IDictionary<object, object> cache,
            DryvOptions options
            );

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
            var parameterOptions = Expression.Parameter(typeof(DryvOptions), "options");

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
                                            parameterCache,
                                            parameterAsyncOnly);

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

        public ValidateAction CreateValidateMethods(Type type)
        {
            var parameterModel = Expression.Parameter(typeof(object), "model");
            var parameterRootModel = Expression.Parameter(typeof(object), "rootModel");
            var parameterServices = Expression.Parameter(typeof(Func<Type, object>), "services");
            var parameterCache = Expression.Parameter(typeof(IDictionary<object, object>), "cache");
            var parameterProcessed = Expression.Parameter(typeof(ICollection<object>), "processed");
            var parameterResult = Expression.Parameter(typeof(IList<DryvResult>), "result");
            var parameterPath = Expression.Parameter(typeof(string), "path");
            var parameterValidator = Expression.Parameter(typeof(DryvValidator), "validator");
            var parameterOptions = Expression.Parameter(typeof(DryvOptions), "options");
            var typedModel = Expression.Convert(parameterModel, type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            var navigationProperties = (from p in properties
                                        where (p.PropertyType.IsClass() || p.PropertyType.IsInterface())
                                              && p.PropertyType != typeof(string)
                                        select p).ToList();

            var returnTarget = Expression.Label();
            var propertyValidationExpressions = from property in properties
                                                let errorVariable = Expression.Parameter(typeof(IEnumerable<DryvResultMessage>), "error")
                                                select (Expression)Expression.Block(
                                                    new[] { errorVariable },
                                                    Expression.Assign(
                                                        errorVariable,
                                                        Expression.Call(
                                                            parameterValidator,
                                                            ValidatePropertyMethod,
                                                            typedModel,
                                                            parameterRootModel,
                                                            Expression.Constant(property),
                                                            parameterServices,
                                                            parameterCache,
                                                            parameterOptions)),
                                                    Expression.IfThen(
                                                        Expression.Not(Expression.Equal(errorVariable, Expression.Constant(null))),
                                                            //Expression.IfThenElse(
                                                            //    Expression.And(
                                                            //        Expression.Equal(Expression.Property(parameterOptions, BreakOnFirstValidationErrorProperty), Expression.Constant(true)),
                                                            //        Expression.GreaterThanOrEqual(Expression.Property(Expression.Convert(parameterResult, typeof(ICollection)), CountProperty), Expression.Constant(1))),
                                                            //    Expression.Return(returnTarget),
                                                            Expression.Call(parameterResult, AddResultMethod,
                                                                Expression.New(ValidationResultCtor,
                                                                    parameterModel,
                                                                    Expression.Constant(property),
                                                                    Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name)),
                                                                    errorVariable)))/*)*/,
                                                    Expression.Label(returnTarget));

            var navigationExpressions = from property in navigationProperties
                                        let childVariable = Expression.Parameter(property.PropertyType, "child")
                                        select Expression.Call(
                                            parameterValidator,
                                            ValidatePathMethod,
                                            Expression.Property(typedModel, property),
                                            parameterRootModel,
                                            Expression.Constant(property),
                                            parameterModel,
                                            Expression.Call(StringConcatMethod, parameterPath, Expression.Constant(property.Name + ".")),
                                            parameterResult,
                                            parameterProcessed,
                                            parameterServices,
                                            parameterCache,
                                            parameterOptions
                                            );

            var lambda = Expression.Lambda<ValidateAction>(
                Expression.Block(propertyValidationExpressions.Union(navigationExpressions)),
                parameterValidator,
                parameterModel,
                parameterRootModel,
                parameterPath,
                parameterResult,
                parameterProcessed,
                parameterServices,
                parameterCache,
                parameterOptions
                );

            return lambda.Compile();
        }
    }
}