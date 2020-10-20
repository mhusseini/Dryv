using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Dryv.Translation.Translators
{
    public class DateTimeTranslator : IDryvCustomTranslator
    {
        public int? OrderIndex { get; set; }
        public bool? AllowSurroundingBrackets(Expression expression)
        {
            return true;
        }

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (ExpressionInjectionHelper.GetInjectionParameters(context.Expression, context) != null ||
                !(context.Expression is BinaryExpression binary))
            {
                return false;
            }

            if ((binary.Left.Type != typeof(DateTime) || binary.Right.Type != typeof(DateTime)) &&
                (binary.Left.Type != typeof(DateTimeOffset) || binary.Right.Type != typeof(DateTimeOffset)) &&
                (binary.Left.Type != typeof(DateTime?) || binary.Right.Type != typeof(DateTime?)) &&
                (binary.Left.Type != typeof(DateTimeOffset?) || binary.Right.Type != typeof(DateTimeOffset?)))
            {
                return false;
            }

            TranslateDate(context, binary.Left);
            context.Writer.Write(" ");
            context.Translator.TryWriteTerminal(context.Expression, context.Writer);
            context.Writer.Write(" ");
            TranslateDate(context, binary.Right);

            return true;
        }

        private static void TranslateDate(TranslationContext context, Expression node)
        {
            var culture = context.Culture;
            var timeZone = node.Type == typeof(DateTimeOffset) || node.Type == typeof(DateTimeOffset?)
                ? " zzz"
                : string.Empty;
            
            var format = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.LongTimePattern}{timeZone}";

            context.Writer.Write("$ctx.dryv.valueOfDate(");
            context.Translator.Translate(node, context);
            context.Writer.Write(",\"");
            context.Writer.Write(culture.Name);
            context.Writer.Write("\",\"");
            context.Writer.Write(MomentJsFormatConverter.ConvertFormat(format));
            context.Writer.Write("\"");
            context.Writer.Write(")");
        }
    }
}