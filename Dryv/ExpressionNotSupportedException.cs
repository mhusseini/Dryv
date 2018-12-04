using System.Linq.Expressions;

namespace Dryv
{
    public class ExpressionNotSupportedException : DryvException
    {
        public Expression Expression { get; }

        public ExpressionNotSupportedException(Expression expression, string message = null)
        : base(message == null
            ? $"The expression'{expression}' is not supported."
            : $"The expression'{expression}' is not supported.: {message}")
        {
            this.Expression = expression;
        }
    }
}