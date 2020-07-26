using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.AspNetCore.DynamicControllers.Runtime
{
    internal class DryvDynamicControllerFilterAttribute : Attribute, IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                return next();
            }

            var error = context.ModelState.First();
            var errorMessage = error.Value.Errors.First().ErrorMessage;
            var index = errorMessage.IndexOf(" Path:", StringComparison.Ordinal);

            context.Result = new JsonResult(new
            {
                text = index < 0 ? errorMessage : errorMessage.Substring(0, index),
                type = "error"
            });

            return Task.CompletedTask;

        }
    }
}