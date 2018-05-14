using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dryv.Utils
{
    internal static class ValidationContextExtensions
    {
        public static PropertyInfo GetProperty(this ValidationContext context)
        {
            return context.ObjectType.GetTypeInfo().GetDeclaredProperty(context.MemberName);
        }

        public static T GetService<T>(this ValidationContext context)
        where T : class
        {
            return context.GetService(typeof(T)) as T;
        }
    }
}