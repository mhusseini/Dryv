using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public class MethodNotSupportedException : DryvException
    {
        public MethodNotSupportedException(MethodCallExpression expression, string message = null)
        : this(expression.Method, message)
        {
        }

        public MethodNotSupportedException(BinaryExpression expression, string message = null)
            : this(expression.Method, message)
        {
        }

        public MethodNotSupportedException(MethodInfo method, string message = null)
            : base($"Calls to the method {method.Name} are not supported. {message ?? string.Empty}")
        {
        }
    }
}