using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv.Utils
{
    internal static class ModelPathExtensions
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

        public static ModelTreeInfo GetTreeInfo(this object model, object root, ValidationContext context)
        {
            return context.Items.GetOrAdd(model, m => GetTreeInfo(m, root));
        }

        public static ModelTreeInfo GetTreeInfo(this object model, object root)
        {
            var x = GetTreeInfo(model, root, ImmutableList<object>.Empty.Add(root), ImmutableList<string>.Empty.Add(string.Empty));
            if (x == null)
            {
                return null;
            }

            var models = x.Value.Item1;
            var paths = x.Value.Item2.Reverse();

            return new ModelTreeInfo
            {
                ModelsByPath = paths
                    .Select((p, i) => new { Key = p, Value = models[i] })
                    .ToDictionary(i => i.Key, i => i.Value),
                PathsByModel = models
                    .Select((m, i) => new { Key = m, Value = paths[i] })
                    .ToDictionary(i => i.Key, i => i.Value)
            };
        }

        private static (ImmutableList<object>, ImmutableList<string>)? GetTreeInfo(object model, object root, ImmutableList<object> models, ImmutableList<string> paths)
        {
            if (root == model)
            {
                return (models, paths);
            }

            var path = paths.Last();
            var pathPrefix = string.IsNullOrWhiteSpace(path) ? string.Empty : ".";
            var rootType = root.GetType();

            foreach (var property in rootType.GetProperties())
            {
                var child = property.GetValue(root);
                if (child == null)
                {
                    continue;
                }

                var paths2 = paths.Add($"{path}{pathPrefix}{property.Name.ToCamelCase()}");
                var models2 = models.Add(child);
                if (child == model)
                {
                    return (models2, paths2);
                }

                if (!property.PropertyType.IsClass || property.PropertyType.Namespace == "System")
                {
                    continue;
                }

                var childResult = GetTreeInfo(model, child, models2, paths2);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            foreach (var property in rootType.GetFields())
            {
                var child = property.GetValue(root);
                if (child == null)
                {
                    continue;
                }

                var paths2 = paths.Add($"{path}{pathPrefix}{property.Name.ToCamelCase()}");
                var models2 = models.Add(child);
                if (child == model)
                {
                    return (models2, paths2);
                }

                if (!property.FieldType.IsClass || property.FieldType.Namespace == "System")
                {
                    continue;
                }

                var childResult = GetTreeInfo(model, child, models2, paths2);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }
    }
}