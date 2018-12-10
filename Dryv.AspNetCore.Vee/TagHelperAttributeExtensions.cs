using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.AspNetCore
{
    internal static class TagHelperAttributeExtensions
    {
        public static void RemoveAll(this ICollection<TagHelperAttribute> allAttributes, IEnumerable<TagHelperAttribute> parameterAttributes)
        {
            foreach (var parameterAttribute in parameterAttributes.Where(a => a != null))
            {
                allAttributes.Remove(parameterAttribute);
            }
        }
    }
}