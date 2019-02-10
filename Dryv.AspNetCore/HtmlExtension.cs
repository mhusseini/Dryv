using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv
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

            var modelType = htmlHelper.ViewData.Model.GetType();
            var modelPath = string.Empty;

            var clientValidation = services.GetService<IDryvClientValidationProvider>().GetValidationCodeForProperty(
                modelType,
                modelPath,
                property,
                services.GetService,
                options.Value);

            return new HtmlString(clientValidation.ValidationFunction);
        }

        public static IEnumerable<DryvClientPropertyValidation> GetDryvClientValidation<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();

            var modelType = htmlHelper.ViewData.Model.GetType();
            var modelPath = string.Empty;

            var validator = services.GetService<IDryvClientValidationProvider>();
            return from property in modelType.GetProperties()
                   let clientValidation = validator.GetValidationCodeForProperty(
                       modelType,
                       modelPath,
                       property,
                       services.GetService,
                       options.Value)
                   where clientValidation != null
                   select clientValidation;
        }
    }
}