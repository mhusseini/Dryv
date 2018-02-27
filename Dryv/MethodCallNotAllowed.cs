using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public class MethodCallNotAllowedException : Exception
    {
        public MethodCallNotAllowedException(MethodCallExpression expression)
        : this(expression.Method)
        {
        }

        public MethodCallNotAllowedException(BinaryExpression expression)
            : this(expression.Method)
        {
        }

        public MethodCallNotAllowedException(MethodInfo method)
            : base($"Calls to the method {method.Name} are not allowed.")
        {
        }
    }
}