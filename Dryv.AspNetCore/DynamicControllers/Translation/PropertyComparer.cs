using System.Collections.Generic;
using System.Reflection;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class PropertyComparer : IEqualityComparer<PropertyInfo>
    {
        public static readonly PropertyComparer Default = new PropertyComparer();

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            return x.DeclaringType.FullName == y.DeclaringType.FullName && x.PropertyType.Name == y.Name;
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return (obj.DeclaringType.FullName + obj.PropertyType.Name).GetHashCode();
        }
    }
}