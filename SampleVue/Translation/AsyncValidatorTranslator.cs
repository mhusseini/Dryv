using System.Text.RegularExpressions;
using Dryv.SampleVue.CustomValidation;
using Dryv.Translation;

namespace Dryv.SampleVue.Translation
{
public class AsyncValidatorTranslator : MethodCallTranslator
{
    public AsyncValidatorTranslator()
    {
        this.Supports<AsyncValidator>();
        this.AddMethodTranslator(new Regex(".*"), TranslateAnyMethod);
    }

    private static void TranslateAnyMethod(MethodTranslationContext context)
    {
        context.Writer.Write("Dryvue.validateAsync('");
        context.Writer.Write(context.Expression.Method.Name);
        context.Writer.Write("', {");

        var sep = string.Empty;
        var method = context.Expression.Method;
        var methodParameters = method.GetParameters();
        var i = 0;

        foreach (var argument in context.Expression.Arguments)
        {
            var parameter = methodParameters[i++];
            context.Writer.Write(sep);
            context.Writer.Write(parameter.Name);
            context.Writer.Write(": ");

            context.Translator.Translate(argument, context);
            sep = ",\n";
        }

        context.Writer.Write("})");
    }
}
}