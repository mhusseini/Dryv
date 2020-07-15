using System;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class DryvValidationAttribute : Attribute
    {
        public DryvValidationAttribute()
        {
        }

        public DryvValidationAttribute(Type ruleContainerType)
        {
            this.RuleContainerType = ruleContainerType;
        }

        public Type RuleContainerType { get; }
    }
}