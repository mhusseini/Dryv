using System;

namespace Dryv.Translation
{
    public class DryvTranslationException : DryvException
    {
        public DryvTranslationException(string message) : this(message, null)
        {
        }

        public DryvTranslationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}