using Dryv.AspNetCore.Utils;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.AspNetCore.TagHelpers
{
    [HtmlTargetElement("dryv-script")]
    public class DryvScriptTagHelper : DryvBodyTagHelperBase
    {
        public DryvScriptTagHelper(IDryvScriptBlockGenerator scriptBlockGenerator)
            : base(scriptBlockGenerator)
        { }

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