using System;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class DryvValidationAttribute : Attribute
    {
    }
}