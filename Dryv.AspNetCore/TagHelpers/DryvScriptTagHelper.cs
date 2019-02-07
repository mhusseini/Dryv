using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.TagHelpers
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
        }
    }
}