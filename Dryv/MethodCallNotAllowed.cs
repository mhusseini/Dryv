using System;
using System.Linq.Expressions;

namespace Dryv
{
    public class MethodCallNotAllowed : Exception
    {
        public MethodCallNotAllowed(MethodCallExpression expression)
            : base($"Calls to the method {expression.Method.Name} are not allowed.")
        {
        }
    }
}