using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dryv
{
    public static class DryvAspNetCoreValidator
    {
        public static async Task<bool> ValidateAsync<TModel>(Controller controller, TModel model)
        {
            var result = true;
            var errors = await DryvValidator.ValidateAsync(model, true, controller.HttpContext.RequestServices.GetService);

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