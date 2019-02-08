using System;

namespace Dryv
{
    public abstract class DryvClientCode
    {
        public static DryvResultMessage CustomMethod(string methodName, params string[] args)
        {
            throw new InvalidOperationException("This constructor is not intended to be called.");
        }
    }
}