using System.Collections.Generic;
using System.Linq.Expressions;
using Dryv.Extensions;

namespace Dryv.Translation
{
    public static class TranslationContextExtensions
    {
        public static string GetVirtualParameter(this TranslationContext context)
        {
            return $"$p{++TranslationContext.ParameterCount}";
        }

        public static bool InjectRuntimeExpression(this TranslationContext context, Expression expression, params ParameterExpression[] parameters)
        {
            return InjectRuntimeExpression(context, expression, (IList<ParameterExpression>) parameters);
        }

        public static bool InjectRuntimeExpression(this TranslationContext context, Expression expression, IList<ParameterExpression> parameters)
        {
            return InjectRuntimeExpression(context, expression, false, parameters);
        }

        public static bool InjectRuntimeExpression(this TranslationContext context, Expression expression, bool isRawOutput, params ParameterExpression[] parameters)
        {
            return InjectRuntimeExpression(context, expression, isRawOutput, (IList<ParameterExpression>) parameters);
        }

        public static bool InjectRuntimeExpression(this TranslationContext context, Expression expression, bool isRawOutput, IList<ParameterExpression> parameters)
        {
            parameters ??= new ParameterExpression[0];

            var canInject = expression is MethodCallExpression mex
                ? ExpressionInjectionHelper.CanInjectMethodCall(mex, context, parameters)
                : ExpressionInjectionHelper.CanInjectProperty(expression, context, parameters);
            
            if (!canInject)
            {
                return false;
            }

            var hash = expression.ToString().GetHashCode();

            if (!context.InjectedExpressions.ContainsKey(hash))
            {
                context.InjectedExpressions.Add(hash, new InjectedExpression
                {
                    LambdaExpression = Expression.Lambda(expression, parameters),
                    IsRawOutput = isRawOutput,
                    Index = hash,
                });
            }

            context.Writer.Write($"$${hash}$$");

            return true;
        }
    }
}