using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dryv.Utils
{
    internal static class ModelTreeInfoExtensions
    {
        public static object FindModel(this ModelTreeInfo tree, string path, ValidationContext context)
        {
            return context.Items.GetOrAdd($"{tree.GetHashCode()}|{path}", _ => tree.FindModel(path));
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
    }
}