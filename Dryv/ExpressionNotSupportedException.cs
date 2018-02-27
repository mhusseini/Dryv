using System;

namespace Dryv
{
    public class ExpressionNotSupportedException : Exception
    {
        public ExpressionNotSupportedException(string message)
            : base(message)
        {
        }
    }
}