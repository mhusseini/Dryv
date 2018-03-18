using System;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    public class AllMethodCallTranslator : MethodCallTranslator
    {
        public AllMethodCallTranslator()
        {
            this.AddMethodTranslator(new Regex(".*", RegexOptions.Compiled), TranslateAnyMethod);
        }

        public override bool SupportsType(Type type) => true;

        private static void TranslateAnyMethod(MethodTranslationContext context)
        {
            context.Translator.VisitWithBrackets(context.Expression.Object, context);
            context.Writer.Write(".");
            context.Writer.Write(context.Expression.Method.Name.ToCamelCase());
            context.Writer.Write("(");
            WriteArguments(context.Translator, context.Expression.Arguments, context);
            context.Writer.Write(")");
        }
    }
}