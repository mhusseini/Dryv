using System;
using System.Text.RegularExpressions;
using Dryv.Extensions;

namespace Dryv.Translation.Translators
{
    public class AllMethodCallTranslator : MethodCallTranslator
    {
        public AllMethodCallTranslator()
        {
            this.AddMethodTranslator(new Regex(".*"), TranslateAnyMethod);
        }

        public override bool SupportsType(Type type) => true;

        private static void TranslateAnyMethod(MethodTranslationContext context)
        {
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".");
            context.Writer.Write(context.Expression.Method.Name.ToCamelCase());
            context.Writer.Write("(");
            WriteArguments(context.Translator, context.Expression.Arguments, context);
            context.Writer.Write(")");
        }
    }
}