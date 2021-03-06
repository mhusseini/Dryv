﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public static List<Type> GetOptionTypes(this LambdaExpression expression)
        {
            var genericArguments = expression.Type.GetGenericArguments();
            return genericArguments
                .Skip(1)
                .Take(genericArguments.Count - 2)
                .ToList();
        }

        public static T GetOuterExpression<T>(this Expression expression)
        where T : Expression
        {
            while (true)
            {
                switch (expression)
                {
                    case T result:
                        return result;

                    case MethodCallExpression methodCallExpression:
                        expression = methodCallExpression.Object;
                        continue;

                    case InvocationExpression invocationExpression:
                        expression = invocationExpression.Expression;
                        continue;

                    case MemberExpression memberExpression:
                        expression = memberExpression.Expression;
                        continue;

                    default:
                        return null;
                }
            }
        }


        /// <summary>
        /// Returns the constant value or compiles and runs the expression and returns the result.
        /// </summary>
        public static object GetValue(this Expression expression)
        {
            return expression switch
            {
                ConstantExpression constantExpression => constantExpression.Value,
                LambdaExpression lambdaExpression => lambdaExpression.Parameters.Count == 0
                    ? CompileAndRun(expression)
                    : throw new ArgumentException("Lambda expression cannot have parameters."),
                _ => CompileAndRun(expression)
            };
        }

        private static object CompileAndRun(Expression expression)
        {
            return Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object))).Compile()();
        }
    }
}