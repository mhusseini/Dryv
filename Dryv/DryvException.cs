using System;

namespace Dryv
{
    public abstract class DryvException : Exception
    {
        protected DryvException(string message) : base(message)
        {
        }
    }
}