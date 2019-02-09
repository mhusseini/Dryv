using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation.Translators
{
    public class DryvResultTranslator : MethodCallTranslator, ICustomTranslator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvResultMessage).GetMember("Success");

        public DryvResultTranslator()
        {
            this.Supports<DryvResultMessage>();
            this.AddMethodTranslator(nameof(DryvResultMessage.Error), Error);
            this.AddMethodTranslator(nameof(DryvResultMessage.Warning), Warning);
            this.AddMethodTranslator(nameof(DryvResultMessage.Success), Success);
        }

        public bool? AllowSurroundingBrackets(Expression expression) => null;

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MemberExpression memberExpression)
                || memberExpression.Member != SuccessMember)
            {
                return false;
            }

            context.Writer.Write("null");
            return true;
        }

        private static void Error(MethodTranslationContext context)
        {
            context.Writer.Write("{ type:'error', message:");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            context.Writer.Write(" }");
        }

        private static void Success(MethodTranslationContext context)
        {
            context.Writer.Write("null");
        }

        private static void Warning(MethodTranslationContext context)
        {
            context.Writer.Write("{ type:'error', warning:");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            context.Writer.Write(" }");
        }
    }
}