using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv
{
    [HtmlTargetElement("body")]
    public class DryvTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync();
            var rulesFileds = from d in this.ViewContext.HttpContext.GetDryvClientRules().Values
                              from rules in d.Values
                              select $"{rules.Name}: {rules.Content}";
            var rulesObject = $@"{{{string.Join(",\r\n", rulesFileds)}}}";
            var scriptContent = new HtmlString($@"<script>window.dryvRules = {rulesObject};</script>");
            output.Content = content.AppendHtml(scriptContent);
        }
    }
}