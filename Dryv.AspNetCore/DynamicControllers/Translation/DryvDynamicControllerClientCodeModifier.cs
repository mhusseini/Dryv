using System.Text;
using System.Text.RegularExpressions;
using Dryv.Translation;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerClientCodeModifier : IDryvClientCodeTransformer
    {
        private static readonly Regex RegexPlaceHolder = new Regex("##route:(.+?)##", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly LinkGenerator linkGenerator;

        public DryvDynamicControllerClientCodeModifier(LinkGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }

        public string Transform(string code)
        {
            var index = 0;
            var sb = new StringBuilder();

            foreach (Match m in RegexPlaceHolder.Matches(code))
            {
                sb.Append(code.Substring(index, m.Index));
                var route = m.Groups[1].Value;
                var url = this.linkGenerator.GetPathByName(route, null);
                sb.Append(url);
                index = m.Index + m.Length;
            }

            sb.Append(code.Substring(index));

            return sb.ToString();
        }
    }
}