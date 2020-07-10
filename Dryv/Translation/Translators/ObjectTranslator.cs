using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Translators
{
    public class ObjectTranslator : ICustomTranslator
    {
        public int? OrderIndex { get; set; }
        public bool? AllowSurroundingBrackets(Expression expression) => false;

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MethodCallExpression methodCallExpression))
            {
                return false;
            }

            if (methodCallExpression.Method.Name != nameof(this.ToString) || methodCallExpression.Arguments.Any())
            {
                return false;
            }

            context.Writer.Write("String(");
            context.Translator.Translate(methodCallExpression.Object, context);
            context.Writer.Write(")");

            return true;
        }
    }
}