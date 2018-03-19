using System.Linq.Expressions;

namespace Dryv
{
    public class ExpressionTypeNotSupportedException : DryvException
    {
        public ExpressionTypeNotSupportedException(Expression expression)
            : base($"The expression type {expression.Type} is not upported.")
        {
        }
    }
}