using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.AspNetCore.Extensions;
using Dryv.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    /// <inheritdoc />
    public class DryvValidationFilterAttribute : Attribute, IAsyncActionFilter
    {
        public Type ParametersProviderType { get; set; }

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
        private async Task<bool> ValidateAsync<TModel>(ActionExecutingContext context, TModel model)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var parameters = GetRuleParametersFromClient(context, serviceProvider);

            var validator = serviceProvider.GetService<DryvValidator>();
            var controller = (Controller)context.Controller;
            var result = true;
            var errors = await validator.Validate(model, serviceProvider.GetService, parameters);
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

        private IReadOnlyDictionary<string, object> GetRuleParametersFromClient(ActionExecutingContext context, IServiceProvider serviceProvider)
        {
            if (this.ParametersProviderType == null)
            {
                return null;
            }

            try
            {
                if (serviceProvider.GetService(this.ParametersProviderType) is IDryParameterProvider parameterProvider)
                {
                    return parameterProvider.GetParameters(context);
                }

                throw new DryvRuntimeException($"The Dryv parameter provider {this.ParametersProviderType.FullName} is not registered with the service provider.");
            }
            catch (Exception ex)
            {
                throw new DryvRuntimeException($"The service provider failed to created a Dryv parameter provider ({this.ParametersProviderType.FullName}).", ex);
            }

        }
    }
}