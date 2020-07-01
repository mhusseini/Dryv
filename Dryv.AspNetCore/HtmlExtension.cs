using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore.Razor;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName);

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName);

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper htmlHelper) => DryvValidation<TModel>(htmlHelper, typeof(TModel).Name.ToCamelCase());

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper) => DryvValidation<TModel>((IHtmlHelper)htmlHelper);

        public static IDictionary<string, IHtmlContent> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientDisablingFunctions<TModel>().ToDictionary(i => i.Key, i => (IHtmlContent)new LazyHtmlContent(i.Value));

        public static IDictionary<string, IHtmlContent> GetDryvClientDisablingFunctions<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientDisablingFunctions<TModel>().ToDictionary(i => i.Key, i => (IHtmlContent)new LazyHtmlContent(i.Value));

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidation<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation<TModel>();

        public static IEnumerable<DryvClientValidationItem> GetDryvClientValidation<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation<TModel>();

        public static DryvClientValidationItem GetDryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression) => GetDryvClientValidationFor((IHtmlHelper)htmlHelper, propertyExpression);

        public static DryvClientValidationItem GetDryvClientValidationFor<TModel>(this IHtmlHelper htmlHelper, Expression<Func<TModel, object>> propertyExpression) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidation(propertyExpression);

        public static IDictionary<string, IHtmlContent> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper<TModel> htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidationFunctions<TModel>().ToDictionary(i => i.Key, i => (IHtmlContent)new LazyHtmlContent(i.Value));

        public static IDictionary<string, IHtmlContent> GetDryvClientValidationFunctions<TModel>(this IHtmlHelper htmlHelper) => htmlHelper.GetDryvClientValidationLoader().GetDryvClientValidationFunctions<TModel>().ToDictionary(i => i.Key, i => (IHtmlContent)new LazyHtmlContent(i.Value));

        private static DryvClientValidationLoader GetDryvClientValidationLoader(this IHtmlHelper htmlHelper) => htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientValidationLoader>();

        private static DryvClientWriter GetDryvClientWriter(this IHtmlHelper htmlHelper) => htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientWriter>();
    }
}