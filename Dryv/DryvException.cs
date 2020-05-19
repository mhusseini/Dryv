using System;

namespace Dryv
{
    public abstract class DryvException : Exception
    {
        protected DryvException(string message, Exception innerException) :
            base(message, innerException)
        { }

        protected DryvException(string message) : this(message, null)
        {
        }
    }
}