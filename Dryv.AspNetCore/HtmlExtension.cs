using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static DryvClientValidationItem DryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression) => DryvClientValidationFor((IHtmlHelper)htmlHelper, propertyExpression);

        public static DryvClientValidationItem DryvClientValidationFor<TModel>(this IHtmlHelper htmlHelper, Expression<Func<TModel, object>> propertyExpression) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation(propertyExpression);

        public static IDictionary<string, string> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientDisablingFunctions<TModel>();

        public static IDictionary<string, string> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientDisablingFunctions<TModel>();

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidation<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation<TModel>();

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidation<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation<TModel>();

        public static IDictionary<string, string> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidationFunctions<TModel>();

        public static IDictionary<string, string> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidationFunctions<TModel>();

        public static IHtmlContent WriteDryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName);

        public static IHtmlContent WriteDryvValidation<TModel>(this IHtmlHelper htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName);

        public static IHtmlContent WriteDryvValidation<TModel>(this IHtmlHelper htmlHelper) => WriteDryvValidation<TModel>(htmlHelper, typeof(TModel).Name.ToCamelCase());

        public static IHtmlContent WriteDryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper) => WriteDryvValidation<TModel>((IHtmlHelper)htmlHelper);

        private static DryvClientValidationLoader GetDryvClientValidationLoader(this IHtmlHelper htmlHelper) => htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();

        private static DryvClientWriter GetDryvClientWriter(this IHtmlHelper htmlHelper) => htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientWriter>();

        private static Func<Type, object> ToServiceFactory(this IHtmlHelper htmlHelper) => t => htmlHelper.ViewContext.HttpContext.RequestServices.GetService(t);
    }
}