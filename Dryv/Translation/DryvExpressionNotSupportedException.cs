using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class DryvExpressionNotSupportedException : DryvException
    {
        public Expression Expression { get; }

        public DryvExpressionNotSupportedException(Expression expression, string message = null)
        : base(message == null
            ? $"The expression'{expression}' is not supported."
            : $"The expression'{expression}' is not supported.: {message}")
        {
            this.Expression = expression;
        }
    }
}