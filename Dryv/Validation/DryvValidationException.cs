using System;
using System.Collections.Generic;
using System.Text;

namespace Dryv.Validation
{
    public class DryvValidationException : DryvException
    {
        public DryvValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DryvValidationException(string message) : base(message)
        {
        }
    }
}