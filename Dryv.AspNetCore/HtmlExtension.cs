using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static DryvClientValidationItem DryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression)
        {
            var loader = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();
            return loader.GetDryvClientValidation<TModel>(propertyExpression);
        }

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidationForModel<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var loader = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();
            return loader.GetDryvClientValidation(typeof(TModel));
        }

        public static IDictionary<string, string> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return (from val in htmlHelper.GetDryvClientValidationForModel()
                    where !string.IsNullOrWhiteSpace(val.ValidationFunction)
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.ValidationFunction);
        }

        public static IDictionary<string, string> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return (from val in htmlHelper.GetDryvClientValidationForModel()
                    where !string.IsNullOrWhiteSpace(val.DisablingFunction)
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.DisablingFunction);
        }
    }
}