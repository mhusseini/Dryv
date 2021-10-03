using System.Collections.Generic;
using System.Linq;
using Dryv.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.Extensions
{
    /// <summary>
    /// Defines methods to overcome some limitations in ASP.NET Core's default model validation.
    /// </summary>
    public static class HttpContextExtensions
    {
        public static IActionResult DryvValidationActionResult(this Controller controller, DryvResultType types = DryvResultType.Error)
        {
            // Dryv integrates into ASP.NET Core's model validation,
            // so you could use this.ModelState here. But since
            // the default model validation does not support warnings,
            // we'll use the extension method GetDryvValidationResults().
            var validationResults = controller.GetDryvValidationResults(types);

            return DryvValidationActionResult(controller, validationResults);
        }

        public static IActionResult DryvValidationActionResult(this Controller controller, IDictionary<string, DryvValidationResult> validationResults)
        {
            return validationResults.Count == 0
                ? controller.Json(new { success = true })
                : controller.Json(new
                {
                    success = false,
                    messages = validationResults
                });
        }

        public static Dictionary<string, DryvValidationResult> GetDryvValidationResults(this Controller controller, DryvResultType types = DryvResultType.Error) => controller.HttpContext.GetDryvValidationResults(types);

        public static Dictionary<string, DryvValidationResult> GetDryvValidationResults(this ActionContext actionContext, DryvResultType types = DryvResultType.Error) => actionContext.HttpContext.GetDryvValidationResults(types);

        public static void SaveDryvValidationResults(this HttpContext httpContext, Dictionary<string, DryvValidationResult> results)
        {
            httpContext.Items.Add(typeof(Dictionary<string, DryvValidationResult>), results);
        }

        public static Dictionary<string, DryvValidationResult> GetDryvValidationResults(this HttpContext httpContext, DryvResultType types = DryvResultType.Error)
        {
            return httpContext.Items.TryGetValue(typeof(Dictionary<string, DryvValidationResult>), out var o) &&
                   o is Dictionary<string, DryvValidationResult> dictionary
                ? (from x in dictionary
                   where types.HasFlag(x.Value.Type)
                   select x)
                .ToDictionary(
                    x => string.Join('.', x.Key.Split('.').Select(v => v.ToCamelCase())),
                    x => x.Value)
                : new Dictionary<string, DryvValidationResult>();
        }
    }
}