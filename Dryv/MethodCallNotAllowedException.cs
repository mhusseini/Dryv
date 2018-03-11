
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public class MethodCallNotAllowedException : Exception
    {
        public MethodCallNotAllowedException(MethodCallExpression expression, string message = null)
        : this(expression.Method, message)
        {
        }

        public MethodCallNotAllowedException(BinaryExpression expression, string message = null)
            : this(expression.Method, message)
        {
        }

        public MethodCallNotAllowedException(MethodInfo method, string message = null)
            : base($"Calls to the method {method.Name} are not allowed. {message ?? string.Empty}")
        {
        }
    }
}