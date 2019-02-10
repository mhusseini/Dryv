using System;

namespace Dryv.Rules
{
    public abstract class DryvClientCode
    {
        public static DryvResultMessage Raw(string script)
        {
            throw new InvalidOperationException("This method is not intended to be called.");
        }
        public static TResult Eaw<TResult>(string script)
        {
            throw new InvalidOperationException("This method is not intended to be called.");
        }
    }
}