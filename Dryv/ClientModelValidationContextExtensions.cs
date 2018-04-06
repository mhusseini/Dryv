using System.Linq;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    internal static class ClientModelValidationContextExtensions
    {
        public static string GetModelPath(this ClientModelValidationContext context)
        {
            context.Attributes.TryGetValue("name", out var modelName);
            if (modelName?.Length == 0)
            {
                modelName = null;
            }

            if (modelName != null)
            {
                modelName = string.Join(".", modelName
                    .Split(".")
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .Select(n => n.ToCamelCase()));
            }

            return modelName;
        }
    }
}