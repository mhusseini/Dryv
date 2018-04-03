using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv.Utils
{
    internal static class ValidationContextExtensions
    {
        public static PropertyInfo GetProperty(this ValidationContext context)
        {
            return context.ObjectType.GetProperty(context.MemberName);
        }
        public static PropertyInfo GetProperty(this ClientModelValidationContext context)
        {
            var metadata = context.ModelMetadata;
            return metadata.ContainerType.GetProperty(metadata.PropertyName);
        }

    }
}