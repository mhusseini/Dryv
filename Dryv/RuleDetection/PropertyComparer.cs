using System.Collections.Generic;
using System.Reflection;

namespace Dryv.RuleDetection
{
    internal class PropertyComparer : IEqualityComparer<PropertyInfo>
    {
        public static readonly PropertyComparer Default = new PropertyComparer();

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            return x != null && y != null && this.GetHashCode(x) == this.GetHashCode(y);
        }

        public int GetHashCode(PropertyInfo property)
        {
            return (property.DeclaringType.FullName?.GetHashCode() ?? 0) ^ property.Name.GetHashCode();
        }
    }
}