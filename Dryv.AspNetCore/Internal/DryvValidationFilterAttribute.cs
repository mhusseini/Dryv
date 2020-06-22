using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Validation;
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
            var model = context.ActionArguments.Values.FirstOrDefault(ShouldValidate);

            if (model != null)
            {
                //await this.ValidateSync((Controller)context.Controller, model);
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
            var resultDictionary = new Dictionary<string, DryvResultMessage>();

            foreach (var x in from error in errors
                              from message in error.Message
                              select new { error, message })
            {
                resultDictionary.Add(x.error.Path, x.message);

                if (x.message.Type != DryvResultType.Error)
                {
                    continue;
                }

                result = false;
                controller.ModelState.AddModelError(x.error.Path, x.message.Text);
            }

            context.HttpContext.Items.Add(typeof(Dictionary<string, DryvResultMessage>), resultDictionary);

            return result;
        }

        ///// <summary>
        ///// Validates the model and sets the model state on the controller accordingly.
        ///// </summary>
        ///// <param name="asyncOnly">Set to true to only validate async rules which otherwise can't (and won't) be run
        ///// inside the synchronous validation infrastructure in ASP.NET core.</param>
        //public async Task<bool> ValidateSync<TModel>(Controller controller, TModel model)
        //{
        //    var result = true;
        //    var errors = await this.validator.Validate(model, this.options.Value, controller.HttpContext.RequestServices.GetService);

        //    foreach (var x in from error in errors
        //                      from message in error.Message
        //                      where message.Type == DryvResultType.Error
        //                      select new { error, message })
        //    {
        //        result = false;
        //        controller.ModelState.AddModelError(x.error.Path, x.message.Text);
        //    }

        //    return result;
        //}

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

        private static bool ShouldValidate(Type t) => GetAllProperties(t).SelectMany(p => p.GetCustomAttributes<DryvRulesAttribute>()).Any();
    }
}