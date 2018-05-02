using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dryv.Utils
{
    internal static class ValidationContextExtensions
    {
        public static PropertyInfo GetProperty(this ValidationContext context)
        {
            return context.ObjectType.GetProperty(context.MemberName);
        }
    }
}