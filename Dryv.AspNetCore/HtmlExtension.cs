using System.Collections.Generic;
using System.Threading.Tasks;
using Dryv.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class HtmlExtensions
    {
        public static Task<IHtmlContent> DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper, string validationSetName, IReadOnlyDictionary<string, object> parameters = null)
        {
            return htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName, htmlHelper.ViewContext.HttpContext.RequestServices.GetService, parameters);
        }

        public static Task<IHtmlContent> DryvValidation<TModel>(this IHtmlHelper htmlHelper, string validationSetName, IReadOnlyDictionary<string, object> parameters = null)
        {
            return htmlHelper.GetDryvClientWriter().WriteDryvValidation<TModel>(validationSetName, htmlHelper.ViewContext.HttpContext.RequestServices.GetService, parameters);
        }

        public static Task<IHtmlContent> DryvValidation<TModel>(this IHtmlHelper htmlHelper, IReadOnlyDictionary<string, object> parameters = null)
        {
            return DryvValidation<TModel>(htmlHelper, typeof(TModel).Name.ToCamelCase(), parameters);
        }

        public static Task<IHtmlContent> DryvValidation<TModel>(this IHtmlHelper<TModel> htmlHelper, IReadOnlyDictionary<string, object> parameters = null)
        {
            return DryvValidation<TModel>((IHtmlHelper)htmlHelper, parameters);
        }

        private static DryvClientWriter GetDryvClientWriter(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetService<DryvClientWriter>();
        }
    }
}