using System;
using System.Reflection;

namespace Dryv.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo member)
        {
            return member switch
            {
                PropertyInfo pi => pi.PropertyType,
                FieldInfo fi => fi.FieldType,
                MethodInfo fi => fi.ReturnType,
            };
        }
    }
}