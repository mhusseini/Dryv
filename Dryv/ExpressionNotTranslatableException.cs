using System;

namespace Dryv
{
    public class ExpressionNotTranslatableException : Exception
    {
        public ExpressionNotTranslatableException(string message) : base(message)
        {
        }
    }
}