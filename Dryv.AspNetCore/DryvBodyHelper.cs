using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv
{
    [HtmlTargetElement("body")]
    public class DryvBodyTagHelper : DryvBodyTagHelperBase
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = this.GetContent();

            if (!string.IsNullOrWhiteSpace(content))
            {
                output
                    .PostContent
                    .AppendHtml(content);
            }

            this.ViewContext.TempData.Remove(ViewContextExtensions.ItemsKey);
        }
    }

    public class DryvBodyTagHelperBase : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected string GetContent()
        {
            var result = this.ViewContext.Load();

            return result.Any()
                ? @"<script>
                    (function(w){
                        var a = w.dryv = w.dryv || {};
                        " + string.Concat(result.Select(i => $"a.{i.Key} = {i.Value};")) + @";
                    })(window);
                </script>"
                : null;
        }
    }

    [HtmlTargetElement("dryv-script")]
    public class DryvScriptTagHelper : DryvBodyTagHelperBase
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = this.GetContent();
            output.TagName = null;

            if (!string.IsNullOrWhiteSpace(content))
            {
                output.Content.AppendHtml(content);
            }

            this.ViewContext.TempData.Remove(ViewContextExtensions.ItemsKey);
        }
    }
}