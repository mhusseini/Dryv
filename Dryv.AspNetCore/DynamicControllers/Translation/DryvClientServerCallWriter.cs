using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;
using Dryv.Translation;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvClientServerCallWriter : IDryvClientServerCallWriter
    {
        public void Write(CustomTranslationContext context, string url, string httpMethod, Dictionary<ParameterInfo, Expression> parameters)
        {
            var w = context.Writer;

            w.Write("dryv.validateAsync('");
            w.Write(url);
            w.Write("', '");
            w.Write(httpMethod);
            w.Write("', {");

            var sep = string.Empty;

            foreach (var (parameter, expression) in parameters)
            {
                w.Write(sep);
                w.Write(parameter.Name.ToCamelCase());
                w.Write(": ");

                context.Translator.Translate(expression, context);

                sep = ",\n";
            }

            w.Write("})");
        }
    }
}