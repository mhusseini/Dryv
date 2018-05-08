using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation
{
    public static class ExpressionExtensions
    {
        private static readonly MethodInfo DefaultMethod;
        private static readonly ConcurrentDictionary<Type, object> DefaultValues = new ConcurrentDictionary<Type, object>();

        static ExpressionExtensions()
        {
            DefaultMethod = typeof(ExpressionExtensions).GetMethods().First(m => m.Name == nameof(GetDefaultValue) && m.IsGenericMethod);
        }

        public static T GetDefaultValue<T>()
        {
            return default(T);
        }

        public static object GetDefaultValue(this Expression expression)
        {
            var expressionType = expression.GetExpressionType();
            return GetDefaultValue(expressionType);
        }

        public static object GetDefaultValue(this Type expressionType)
        {
            return DefaultValues.GetOrAdd(expressionType, type => DefaultMethod.MakeGenericMethod(type).Invoke(null, null));
        }

        public static Type GetExpressionType(this Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression methodCallExpression:
                    return methodCallExpression.Method.ReturnType;

                case ConditionalExpression conditionalExpression:
                    return GetExpressionType(conditionalExpression.IfTrue);

                case InvocationExpression invocationExpression:
                    return invocationExpression.Type;

                case LambdaExpression lambdaExpression:
                    return lambdaExpression.ReturnType;

                case MemberExpression memberExpression:
                    return memberExpression.Type;

                case NewExpression newExpression:
                    return newExpression.Type;

                case ParameterExpression parameterExpression:
                    return parameterExpression.Type;

                case ConstantExpression constantExpression:
                    return constantExpression.Type;

                default:
                    return typeof(object);
            }
        }
    }
}