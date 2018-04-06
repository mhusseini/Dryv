using System.Linq;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    internal static class ModelPathExtensions
    {
        public static string FindPathOn(this object model, object root)
        {
            return FindPathOn(model, root, null);
        }

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

        private static string FindPathOn(object model, object root, string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }

            if (root == model)
            {
                return path;
            }

            var pathPrefix = string.IsNullOrWhiteSpace(path) ? string.Empty : ".";
            var rootType = root.GetType();

            foreach (var property in rootType.GetProperties())
            {
                var path2 = $"{path}{pathPrefix}{property.Name.ToCamelCase()}";
                var child = property.GetValue(root);
                if (child == model)
                {
                    return path2;
                }

                if (!property.PropertyType.IsClass || property.PropertyType.Namespace == "System")
                {
                    continue;
                }

                var childResult = FindPathOn(model, child, path2);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            foreach (var property in rootType.GetFields())
            {
                var path2 = $"{path}{pathPrefix}{property.Name.ToCamelCase()}";
                var child = property.GetValue(root);
                if (child == model)
                {
                    return path2;
                }

                if (!property.FieldType.IsClass || property.FieldType.Namespace == "System")
                {
                    continue;
                }

                var childResult = FindPathOn(model, child, path2);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }
    }
}