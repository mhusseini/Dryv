using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv.MethodCallTranslation
{
    internal class DryvResultTranslator : MethodCallTranslator, IGenericTranslator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvResult).GetMember("Success").First();

        public DryvResultTranslator()
        {
            this.Supports<DryvResult>();
            this.AddMethodTranslator(nameof(DryvResult.Fail), this.Error);
        }

        private void Error(MethodTranslationParameters parameters)
        {
            parameters.Writer.Write(parameters.Expression.Arguments.First());
        }

        public bool TryTranslate(TranslationParameters parameters)
        {
            if (!(parameters.Expression is MemberExpression memberExpression)
                || memberExpression.Member != SuccessMember)
            {
                return false;
            }

            parameters.Writer.Write("null");
            return true;
        }
    }
}