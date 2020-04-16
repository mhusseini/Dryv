using System.Linq.Expressions;
using System.Reflection;

namespace Dryv.Translation
{
    public class DryvMethodNotSupportedException : DryvException
    {
        public DryvMethodNotSupportedException(MethodCallExpression expression, string message = null)
        : this(expression.Method, message)
        {
        }

        public DryvMethodNotSupportedException(BinaryExpression expression, string message = null)
            : this(expression.Method, message)
        {
        }

        public DryvMethodNotSupportedException(MethodInfo method, string message = null)
            : base($"Calls to the method {method.Name} are not supported. {message ?? string.Empty}")
        {
        }
    }
}