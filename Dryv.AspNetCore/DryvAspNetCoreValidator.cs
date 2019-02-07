using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dryv
{
    /// <summary>
    /// Provides ASP.NET Core-specific access to Dryv validation.
    /// </summary>
    public static class DryvAspNetCoreValidator
    {
        /// <summary>
        /// Validates the model and sets the model state on the controller accordingly.
        /// </summary>
        /// <param name="asyncOnly">Set to true to only validate async rules which otherwise can't (and won't) be run
        /// inside the synchronous validation infrastructure in ASP.NET core.</param>
        public static async Task<bool> ValidateAsync<TModel>(Controller controller, TModel model, bool asyncOnly = false)
        {
            var result = true;
            var errors = await DryvValidator.ValidateAsync(model, asyncOnly, controller.HttpContext.RequestServices.GetService);

            foreach (var x in from error in errors
                              from message in error.Message
                              where message.Type == DryvResultType.Error
                              select new { error, message })
            {
                result = false;
                controller.ModelState.AddModelError(x.error.Path, x.message.Text);
            }

            return result;
        }
    }
}