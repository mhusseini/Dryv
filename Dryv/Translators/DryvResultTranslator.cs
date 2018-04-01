using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.Translators
{
    internal class DryvResultTranslator : MethodCallTranslator, ICustomTranslator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvResult).GetMember("Success").First();

        public DryvResultTranslator()
        {
            this.Supports<DryvResult>();
            this.AddMethodTranslator(nameof(DryvResult.Error), Error);
            this.AddMethodTranslator(nameof(DryvResult.Warning), Warning);
            this.AddMethodTranslator(nameof(DryvResult.Success), Success);
        }

        private static void Error(MethodTranslationContext context)
        {
            context.Writer.Write($"{{ type:'error', message:'{context.Expression.Arguments.First()}' }}");
        }

        private static void Warning(MethodTranslationContext context)
        {
            context.Writer.Write($"{{ type:'warning', message:'{context.Expression.Arguments.First()}' }}");
        }

        private static void Success(MethodTranslationContext context)
        {
            context.Writer.Write("null");
        }

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MemberExpression memberExpression)
                || memberExpression.Member != SuccessMember)
            {
                return false;
            }

            context.Writer.Write("null");
            return true;
        }
    }
}