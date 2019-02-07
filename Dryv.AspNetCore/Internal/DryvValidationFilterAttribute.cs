using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.Internal
{
    /// <inheritdoc />
    /// <summary>
    /// As long as mvc validation is not async, we'll 
    /// run the async validation from an action attribute.
    /// </summary>
    internal class DryvValidationFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var model = context.ActionArguments.Values.FirstOrDefault(ShouldValidate);

            if (model != null)
            {
                await DryvAspNetCoreValidator.ValidateAsync((Controller)context.Controller, model, true);
            }

            await next();
        }

        private static bool ShouldValidate(object a) => ShouldValidate(a?.GetType());

        private static bool ShouldValidate(Type t) => t?.IsPrimitive == false
                                                      && t != typeof(string)
                                                      && t.GetProperties().Any(p => p.GetCustomAttributes<DryvRulesAttribute>().Any());
    }
}