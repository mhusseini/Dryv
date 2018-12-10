using System.Linq;
using System.Reflection;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dryv.AspNetCore.Utils
{
    internal static class ModelPathExtensions
    {
        public static PropertyInfo GetProperty(this ModelExpression model)
        {
            var metadata = model.Metadata;
            return metadata.ContainerType.GetProperty(metadata.PropertyName);
        }

        public static string GetModelPath(this ViewContext viewContext, ModelExpression model)
        {
            var propertyPath = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(model.Name);

            if (propertyPath?.Length == 0)
            {
                propertyPath = null;
            }

            if (propertyPath != null)
            {
                return string.Join(".", propertyPath
                    .Split(".")
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .Select(n => n.ToCamelCase()));
            }

            return null;
        }
    }
}