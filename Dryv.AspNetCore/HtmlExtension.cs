using System;
using System.Collections.Generic;
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

        public static IEnumerable<DryvClientValidationItem> GetDryvClientPropertyValidations<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var loader = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();
            return loader.GetDryvClientValidation(typeof(TModel));
        }
    }
}