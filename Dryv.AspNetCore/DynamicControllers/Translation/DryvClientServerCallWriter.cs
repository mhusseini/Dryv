using System.Collections.Generic;
using System.Linq.Expressions;
using Dryv.Extensions;
using Dryv.Translation;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvClientServerCallWriter : IDryvClientServerCallWriter
    {
        public void Write(TranslationContext context, ITranslator translator, string url, string httpMethod, IList<MemberExpression> members)
        {
            var w = context.Writer;

            w.Write("dryv.validateAsync('");
            w.Write(url);
            w.Write("','");
            w.Write(httpMethod);
            w.Write("',{");

            var sep = string.Empty;

            foreach (var memberExpression in members)
            {
                w.Write(sep);
                w.Write(memberExpression.Member.Name.ToCamelCase());
                w.Write(":");

                translator.Translate(memberExpression, context);

                sep = ",";
            }

            w.Write("})");
        }
    }
}