using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public static class TranslationContextExtensions
    {
        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, params ParameterExpression[] parameters)
        {
            InjectRuntimeExpression(context, expression, false, parameters);
        }

        public static void InjectRuntimeExpression(this TranslationContext context, Expression expression, bool isRawOutput, params ParameterExpression[] parameters)
        {
            if (!parameters.Any())
            {
                parameters = expression is MethodCallExpression methodCallExpression
                    ? methodCallExpression.Arguments.Select(a => a.GetOuterExpression<ParameterExpression>()).ToArray()
                    : new ParameterExpression[0];
            }

            var hash = expression.ToString().GetHashCode();

            if (!context.OptionDelegates.ContainsKey(hash))
            {
                context.OptionDelegates.Add(hash, new OptionDelegate
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