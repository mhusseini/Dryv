using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DefaultDryvDynamicControllerCallWriter : IDryvDynamicControllerCallWriter
    {
        public void Write(CustomTranslationContext context, string url, string httpMethod, Dictionary<ParameterInfo, Expression> parameters)
        {
            context.Writer.Write("Dryvue.validateAsync('");
            context.Writer.Write(url);
            context.Writer.Write("', '");
            context.Writer.Write(httpMethod);
            context.Writer.Write("', {");

            var sep = string.Empty;

            foreach (var (parameter, expression) in parameters)
            {
                context.Writer.Write(sep);
                context.Writer.Write(parameter.Name);
                context.Writer.Write(": ");

                context.Translator.Translate(expression, context);
                sep = ",\n";
            }

            context.Writer.Write("})");
        }
    }
}