using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.AspNetCore.Extensions;
using Dryv.Rework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dryv.AspNetCore.Internal
{
    /// <inheritdoc />
    /// <summary>
    /// As long as mvc validation is not async, we'll
    /// run the async validation from an action attribute.
    /// </summary>
    internal class DryvValidationFilterAttribute : IAsyncActionFilter
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

            var model = context.ActionArguments.Values.FirstOrDefault(ShouldValidate);

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

        private static IEnumerable<PropertyInfo> GetAllProperties(Type type) => GetAllProperties(type, new HashSet<PropertyInfo>());

        private static void GetAllProperties(PropertyInfo property, HashSet<PropertyInfo> list)
        {
            if (list.Contains(property))
            {
                return;
            }

            list.Add(property);

            GetAllProperties(property.PropertyType, list);
        }

        private static HashSet<PropertyInfo> GetAllProperties(Type type, HashSet<PropertyInfo> list)
        {
            if (IsPrimitive(type))
            {
                return list;
            }

            foreach (var p in type.GetProperties())
            {
                GetAllProperties(p, list);
            }

            return list;
        }

        private static bool IsPrimitive(Type t) => t.IsPrimitive || t == typeof(string);

        private static bool ShouldValidate(object a) => ShouldValidate(a?.GetType());

        private static bool ShouldValidate(Type t) => GetAllProperties(t).SelectMany(p => p.GetCustomAttributes<DryvValidationAttribute>()).Any();
    }
}