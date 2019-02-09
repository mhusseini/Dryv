using System;

namespace Dryv
{
    public abstract class DryvClientCode
    {
        public static DryvResultMessage CustomScript(string script)
        {
            throw new InvalidOperationException("This method is not intended to be called.");
        }
        public static TResult CustomScript<TResult>(string script)
        {
            throw new InvalidOperationException("This method is not intended to be called.");
        }
    }
}