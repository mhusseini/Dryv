using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dryv.Reflection;

namespace Dryv.Utils
{
    internal static class ModelTreeInfoExtensions
    {
        public static object FindModel(this ModelTreeInfo tree, string path, object currentModel, IDictionary<object, object> cache)
        {
            var value = cache.GetOrAdd($"{tree.GetHashCode()}|{path}", _ => tree.FindModel(path));
            return !(value is string)
                   && value is IEnumerable enumerable
                   && enumerable.Cast<object>().Contains(currentModel)
                ? currentModel
                : value;
        }

        public static object FindModel(this ModelTreeInfo tree, string path)
        {
            if (tree.ModelsByPath.TryGetValue(path, out var model))
            {
                return model;
            }

            var orderedItems = tree.ModelsByPath.OrderByDescending(i => i.Key.Length).ToList();
            var current = orderedItems.First();
            var k = current.Key.Substring(0, current.Key.Length - path.Length);
            if (k.EndsWith("."))
            {
                k = k.Substring(0, k.Length - 1);
            }

            return orderedItems.FirstOrDefault(i => i.Key == k).Value;
        }

        public static ModelTreeInfo GetTreeInfo(this object model, object root, IDictionary<object, object> cache)
        {
            return cache.GetOrAdd(model, m => GetTreeInfo(m, root));
        }

        public static ModelTreeInfo GetTreeInfo(this object model, object root)
        {
            var x = GetTreeInfo(model, root, ImmutableList<object>.Empty.Add(root), ImmutableList<string>.Empty.Add(string.Empty));
            if (x == null)
            {
                return null;
            }

            var models = x.Models;
            var paths = x.Paths.Reverse();

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

        private static TreeInfo GetTreeInfo(object model, object root, ImmutableList<object> models, ImmutableList<string> paths)
        {
            if (root == model)
            {
                return new TreeInfo(models, paths);
            }

            var path = paths.Last();
            var pathPrefix = string.IsNullOrWhiteSpace(path) ? string.Empty : ".";
            var rootType = root.GetType();

            if (!(root is string) && root is IList list)
            {
                var models2 = models.Add(model);

                var index = list.IndexOf(model);
                if (index >= 0)
                {
                    return new TreeInfo(models2, paths.Add($"{path}[{index}]"));
                }

                index = 0;
                foreach (var child in list)
                {
                    var childResult = GetTreeInfo(model, child, models2, paths.Add($"{path}[{index++}]"));
                    if (childResult != null)
                    {
                        return childResult;
                    }
                }

                return null;
            }

            foreach (var property in rootType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
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
                    return new TreeInfo(models2, paths2);
                }

                if (!(child is string) && child is IEnumerable enumerable && enumerable.Cast<object>().Contains(model))
                {
                    return new TreeInfo(models2, paths2);
                }

                if (!property.PropertyType.IsClass() || property.PropertyType.Namespace == "System")
                {
                    continue;
                }

                var childResult = GetTreeInfo(model, child, models2, paths2);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            foreach (var property in rootType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
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
                    return new TreeInfo(models2, paths2);
                }

                if (!property.FieldType.IsClass() || property.FieldType.Namespace == "System")
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

        private class TreeInfo
        {
            public TreeInfo(ImmutableList<object> models, ImmutableList<string> paths)
            {
                this.Models = models;
                this.Paths = paths;
            }

            public ImmutableList<object> Models { get; }

            public ImmutableList<string> Paths { get; }
        }
    }
}