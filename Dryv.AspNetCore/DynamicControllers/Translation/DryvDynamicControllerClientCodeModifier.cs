using System.Text;
using System.Text.RegularExpressions;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Dryv.DynamicControllers.Translation
{
    internal class DryvDynamicControllerClientCodeModifier : IDryvClientCodeModifier
    {
        private static readonly Regex RegexPlaceHolder = new Regex("##route:(.+?)##", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly IUrlHelperFactory urlHelperFactory;

        public DryvDynamicControllerClientCodeModifier(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            this.actionContextAccessor = actionContextAccessor;
            this.urlHelperFactory = urlHelperFactory;
        }

        public string Transform(string code)
        {
            var actionContext = this.actionContextAccessor.ActionContext;
            var urlHelper = this.urlHelperFactory.GetUrlHelper(actionContext);

            var index = 0;
            var sb = new StringBuilder();

            foreach (Match m in RegexPlaceHolder.Matches(code))
            {
                sb.Append(code.Substring(index, m.Index));
                var route = m.Groups[1].Value;
                var url = urlHelper.RouteUrl(route);
                sb.Append(url);
                index = m.Index + m.Length;
            }

            sb.Append(code.Substring(index));

            return sb.ToString();
        }
    }
}