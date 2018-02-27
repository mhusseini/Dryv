using System;
using System.Linq.Expressions;

namespace Dryv
{
    public class ExpressionTypeNotSupportedException : Exception
    {
        public ExpressionTypeNotSupportedException(Expression expression)
            : base($"The expression type {expression.Type} is not upported.")
        {
        }
    }
}