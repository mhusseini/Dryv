using System;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Class)]
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