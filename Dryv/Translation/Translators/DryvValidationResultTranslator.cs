using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation.Translators
{
    public class DryvValidationResultTranslator : MethodCallTranslator, ICustomTranslator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvValidationResult).GetMember("Success");

        public DryvValidationResultTranslator()
        {
            this.Supports<DryvValidationResult>();
            this.AddMethodTranslator(nameof(DryvValidationResult.Error), Error);
            this.AddMethodTranslator(nameof(DryvValidationResult.Warning), Warning);
            this.AddMethodTranslator(nameof(DryvValidationResult.Success), Success);
        }

        public bool? AllowSurroundingBrackets(Expression expression) => null;

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MemberExpression memberExpression)
                || !Equals(memberExpression.Member, SuccessMember))
            {
                return false;
            }

            context.Writer.Write("null");
            return true;
        }

        private static void Error(MethodTranslationContext context)
        {
            context.Writer.Write("{ type:\"error\", text:");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            if (!string.IsNullOrWhiteSpace(context.GroupName))
            {
                context.Writer.Write(", groupName: ");
                context.Writer.Write(QuoteValue(context.GroupName));
            }
            context.Writer.Write(" }");
        }

        private static void Success(MethodTranslationContext context)
        {
            context.Writer.Write("null");
        }

        private static void Warning(MethodTranslationContext context)
        {
            context.Writer.Write("{ type:\"warning\", text:");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            if (!string.IsNullOrWhiteSpace(context.GroupName))
            {
                context.Writer.Write(", groupName: ");
                context.Writer.Write(QuoteValue(context.GroupName));
            }
            context.Writer.Write(" }");
        }
    }
}