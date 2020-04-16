using System;
using System.Collections.Generic;
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