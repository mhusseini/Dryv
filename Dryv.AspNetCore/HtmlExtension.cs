using System;
using System.Collections.Generic;
using System.IO;
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
            return loader.GetDryvClientValidation(propertyExpression);
        }

        public static IDictionary<string, Action<TextWriter>> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return (from val in htmlHelper.GetDryvClientValidationForModel()
                    where val.DisablingFunction != null
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.DisablingFunction);
        }

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidationForModel<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var loader = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();
            return loader.GetDryvClientValidation(typeof(TModel));
        }

        public static IDictionary<string, Action<TextWriter>> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return (from val in htmlHelper.GetDryvClientValidationForModel()
                    where val.ValidationFunction != null
                    select val)
                .ToDictionary(
                    val => val.ModelPath + (string.IsNullOrWhiteSpace(val.ModelPath) ? string.Empty : ".") + val.Property.Name.ToCamelCase(),
                    val => val.ValidationFunction);
        }
    }
}