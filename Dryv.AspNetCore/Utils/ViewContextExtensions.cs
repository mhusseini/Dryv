using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dryv.Utils
{
    internal static class ViewContextExtensions
    {
        private static readonly IDictionary<string, DryvClientPropertyValidation> EmptyValidation = new ReadOnlyDictionary<string, DryvClientPropertyValidation>(new Dictionary<string, DryvClientPropertyValidation>());

        public static IDictionary<string, DryvClientPropertyValidation> LoadValidationCode(this ViewContext viewContext)
        {
            return viewContext.HttpContext.Features.Get<DryvFeature>()?.PropertyValidations ?? EmptyValidation;
        }

        public static IDictionary<string, DryvClientPropertyValidation> PopValidationCode(this ViewContext viewContext)
        {
            var result = LoadValidationCode(viewContext);
            if (result != null)
            {
                result = result.ToDictionary(i => i.Key, i => i.Value);
                viewContext.HttpContext.Features.Get<DryvFeature>()?.PropertyValidations.Clear();
            }

            return result;
        }

        public static void StoreValidationCode(this ViewContext viewContext, string key, DryvClientPropertyValidation value)
        {
            var feature = viewContext.HttpContext.Features.Get<DryvFeature>();
            if (feature == null)
            {
                feature = new DryvFeature();
                viewContext.HttpContext.Features.Set(feature);
            }

            feature.PropertyValidations[key] = value;
        }
    }
}