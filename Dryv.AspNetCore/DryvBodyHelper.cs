using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv
{
    [HtmlTargetElement("body")]
    public class DryvBodyHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var result = this.ViewContext.Load();

            output
                .PostContent
                .Append("<script>")
                .Append("window.dryv = {" + string.Join(",", result.Select(i => $"{i.Key}: {i.Value}")) + "};")
                .Append("</script>");

            return Task.CompletedTask;
        }
    }
}