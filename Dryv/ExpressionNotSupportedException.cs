using System.Linq.Expressions;

namespace Dryv
{
    public class ExpressionNotSupportedException : DryvException
    {
        public ExpressionNotSupportedException(Expression expression)
        : this($"The expression'{expression}' is not supported.")
        {

        }
        public ExpressionNotSupportedException(string message)
            : base(message)
        {
        }
    }
}