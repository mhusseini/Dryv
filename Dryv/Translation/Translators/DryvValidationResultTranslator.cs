using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation.Translators
{
    public class DryvValidationResultTranslator : MethodCallTranslator, IDryvCustomTranslator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvValidationResult).GetMember("Success");

        public DryvValidationResultTranslator()
        {
            this.Supports<DryvValidationResult>();
            this.AddMethodTranslator(nameof(DryvValidationResult.Error), ctx => Translate(ctx, "error"));
            this.AddMethodTranslator(nameof(DryvValidationResult.Warning), ctx => Translate(ctx, "warning"));
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

        private static void Success(MethodTranslationContext context)
        {
            context.Writer.Write("null");
        }

        private static void Translate(MethodTranslationContext context, string resultType)
        {
            var data = context.Expression.Arguments.FirstOrDefault(a => a.Type == typeof(object));

            context.Writer.Write("{ type:\"");
            context.Writer.Write(resultType);
            context.Writer.Write("\", text:");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            if (!string.IsNullOrWhiteSpace(context.Group))
            {
                context.Writer.Write(", group: ");
                context.Writer.Write(JavaScriptHelper.TranslateValue(context.Group));
            }

            if (data != null)
            {
                context.Writer.Write(", data: ");
                context.Translator.Translate(data, context);
            }

            context.Writer.Write(" }");
        }
    }
}