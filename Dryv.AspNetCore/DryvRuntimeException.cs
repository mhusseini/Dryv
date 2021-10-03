using System;

namespace Dryv.AspNetCore
{
    public class DryvRuntimeException : Exception
    {
        public DryvRuntimeException(string message) : base(message)
        {
        }
        
        public DryvRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}