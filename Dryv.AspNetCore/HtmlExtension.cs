using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static IHtmlContent DryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"The parameter '{nameof(propertyExpression)}' must be a property.");
            }

            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();
            var clientValidation = services
                .GetService<IDryvClientValidationProvider>()
                .GetClientPropertyValidation(htmlHelper.ViewData.Model.GetType(), string.Empty, property, services.GetService, options.Value);

            return new HtmlString(clientValidation.ValidationFunction);
        }

        public static IEnumerable<DryvClientValidationItem> GetDryvClientPropertyValidations<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var modelType = typeof(TModel);
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();
            var validator = services.GetService<IDryvClientValidationProvider>();

            return CollectClientValidation(modelType, modelType, string.Empty, validator, new Stack<string>(), options.Value, services.GetService);
        }

        internal static IEnumerable<DryvClientValidationItem> CollectClientValidation(Type modelType, Type rootModelType, string modelPath, IDryvClientValidationProvider validator, Stack<string> processedTypes, DryvOptions options, Func<Type, object> services)
        {
            if (modelType == typeof(Type) ||
                modelType.Namespace?.StartsWith("System.Reflection") == true ||
                modelType.FullName == null ||
                processedTypes.Contains(modelType.FullName))
            {
                return new DryvClientValidationItem[0];
            }

            var properties = modelType.GetProperties();

            var items = from property in properties
                        let item = validator.GetClientPropertyValidation(modelType, modelPath, property, services, options)
                        where item != null
                        select item;

            processedTypes.Push(modelType.FullName);

            var childItems = from property in properties
                             let prefix = string.IsNullOrWhiteSpace(modelPath) ? string.Empty : $"{modelPath}."
                             let childPath = $"{prefix}{property.Name.ToCamelCase()}"
                             where !property.PropertyType.IsValueType && !property.PropertyType.HasElementType && property.PropertyType != typeof(string) && !processedTypes.Contains(childPath)
                             from item in CollectClientValidation(property.PropertyType, rootModelType, childPath, validator, processedTypes, options, services)
                             select item;

            processedTypes.Pop();

            return items.Union(childItems);
        }
    }
}