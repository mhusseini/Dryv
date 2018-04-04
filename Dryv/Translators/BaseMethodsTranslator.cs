using System.Linq;
using System.Linq.Expressions;
using Dryv.Translation;

namespace Dryv.Translators
{
    internal class BaseMethodsTranslator : ICustomTranslator
    {
        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MethodCallExpression methodCallExpression))
            {
                return false;
            }

            if (methodCallExpression.Method.Name != nameof(object.ToString) || methodCallExpression.Arguments.Any())
            {
                return false;
            }

            context.Writer.Write("String(");
            context.Translator.Visit(methodCallExpression.Object, context);
            context.Writer.Write(")");

            return true;
        }
    }
}