using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Translators
{
    public class CustomCodeTranslator : MethodCallTranslator
    {
        public CustomCodeTranslator()
        {
            this.Supports<DryvClientCode>();
            this.AddMethodTranslator(nameof(DryvClientCode.CustomMethod), CustomMethod);
        }

        private static void CustomMethod(MethodTranslationContext context)
        {
            var script = context.Expression.Arguments.First();

            context.InjectRuntimeExpression(script, true);
            context.Writer.Write("(");
            if (context.Expression.Arguments.Skip(1).FirstOrDefault() is NewArrayExpression array)
            {
                WriteArguments(context.Translator, array.Expressions, context);
            }
            context.Writer.Write(")");
        }
    }
}