using Dryv.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName, htmlHelper.ViewContext.HttpContext.RequestServices.GetService);

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper htmlHelper, string validationSetName) => htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName, htmlHelper.ViewContext.HttpContext.RequestServices.GetService);

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper htmlHelper) => DryvValidation<TModel>(htmlHelper, typeof(TModel).Name.ToCamelCase());

        public static IHtmlContent DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper) => DryvValidation<TModel>((IHtmlHelper)htmlHelper);

        private static DryvClientWriter GetDryvClientWriter(this IHtmlHelper htmlHelper) => htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientWriter>();
    }
}