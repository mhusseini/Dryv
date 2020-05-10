using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dryv.AspNetCore.Internal;
using Dryv.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dryv.AspNetCore.Extensions
{
    internal static class ViewContextExtensions
    {
        private static readonly IDictionary<string, DryvClientValidationItem> EmptyValidation = new ReadOnlyDictionary<string, DryvClientValidationItem>(new Dictionary<string, DryvClientValidationItem>());

        public static DryvFeature GetDryvFeature(this HttpContext httpContext)
        {
            var feature = httpContext.Features.Get<DryvFeature>();
            if (feature == null)
            {
                feature = new DryvFeature();
                httpContext.Features.Set(feature);
            }

            return feature;
        }

        public static IDictionary<string, DryvClientValidationItem> LoadValidationCode(this ViewContext viewContext)
        {
            return viewContext.HttpContext.Features.Get<DryvFeature>()?.PropertyValidations ?? EmptyValidation;
        }

        public static IDictionary<string, DryvClientValidationItem> PopValidationCode(this ViewContext viewContext)
        {
            var result = LoadValidationCode(viewContext);
            if (result != null)
            {
                result = result.ToDictionary(i => i.Key, i => i.Value);
                viewContext.HttpContext.Features.Get<DryvFeature>()?.PropertyValidations.Clear();
            }

            return result;
        }

        public static void StoreValidationCode(this ViewContext viewContext, string key, DryvClientValidationItem value)
        {
            viewContext.HttpContext.GetDryvFeature().PropertyValidations[key] = value;
        }
    }
}