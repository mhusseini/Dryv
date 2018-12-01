using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dryv
{
    internal static class ViewContextExtensions
    {
        internal const string ItemsKey = "dryv";

        public static IDictionary<string, string> Load(this ViewContext viewContext)
        {
            return viewContext.TempData.TryGetValue(ItemsKey, out var o) && o is DryvTagHelperResult result
                ? result.ValidationFunctions
                : new Dictionary<string, string>();
        }

        public static void Store(this ViewContext viewContext, string key, string value)
        {
            if (!viewContext.TempData.TryGetValue(ItemsKey, out var o) || !(o is DryvTagHelperResult result))
            {
                result = new DryvTagHelperResult();
                viewContext.TempData.Add(ItemsKey, result);
            }

            result.ValidationFunctions[key] = value;
        }
    }
}