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

        private static void TranslateAnyMethod(MethodTranslationParameters parameters)
        {
            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".");
            parameters.Writer.Write(parameters.Expression.Method.Name.ToCamelCase());
            parameters.Writer.Write("(");
            WriteArguments(parameters.Translator, parameters.Expression.Arguments, parameters.Writer);
            parameters.Writer.Write(")");
        }
    }
}