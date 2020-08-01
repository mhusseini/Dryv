using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.Extensions;
using Dryv.Translation.Visitors;

namespace Dryv.Translation
{
    public static class TranslationContextExtensions
    {
        public static string GetVirtualParameter(this TranslationContext context)
        {
            return $"$p{++TranslationContext.ParameterCount}";
        }

        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, params ParameterExpression[] parameters)
        {
            InjectRuntimeExpression(context, expression, (IList<ParameterExpression>)parameters);
        }

        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, IList<ParameterExpression> parameters)
        {
            InjectRuntimeExpression(context, expression, false, parameters);
        }

        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, bool isRawOutput, params ParameterExpression[] parameters)
        {
            InjectRuntimeExpression(context, expression, isRawOutput, (IList<ParameterExpression>)parameters);
        }

        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, bool isRawOutput, IList<ParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                parameters = ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic(expression);
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
        }
    }
}