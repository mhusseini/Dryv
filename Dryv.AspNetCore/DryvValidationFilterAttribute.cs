using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.AspNetCore
{
    /// <inheritdoc />
    public class DryvValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly DryvValidator validator;

        public DryvValidationFilterAttribute(DryvValidator validator)
        {
            this.validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Filters.OfType<DryvDisableAttribute>().Any())
            {
                await next();

                return;
            }

            var model = context.ActionArguments.Values.FirstOrDefault();

            if (model != null)
            {
                await this.ValidateAsync(context, model);
            }

            await next();
        }

        /// <summary>
        /// Validates the model and sets the model state on the controller accordingly.
        /// </summary>
        public async Task<bool> ValidateAsync<TModel>(ActionExecutingContext context, TModel model)
        {
            var controller = (Controller)context.Controller;
            var result = true;
            var errors = await this.validator.Validate(model, controller.HttpContext.RequestServices.GetService);
            var resultDictionary = new Dictionary<string, DryvValidationResult>();

            foreach (var error in errors)
            {
                resultDictionary.Add(error.Key, error.Value);

                if (error.Value.Type != DryvResultType.Error)
                {
                    continue;
                }

                result = false;
                controller.ModelState.AddModelError(error.Key, error.Value.Text);
            }

            context.HttpContext.SaveDryvValidationResults(resultDictionary);

            return result;
        }
    }
}