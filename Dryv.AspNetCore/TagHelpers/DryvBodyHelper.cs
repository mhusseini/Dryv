using Dryv.Translation;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.TagHelpers
{
    [HtmlTargetElement("body")]
    public class DryvBodyTagHelper : DryvBodyTagHelperBase
    {
        public DryvBodyTagHelper(IDryvScriptBlockGenerator scriptBlockGenerator)
        : base(scriptBlockGenerator)
        { }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = this.GetContent();

            if (!string.IsNullOrWhiteSpace(content))
            {
                output
                    .PostContent
                    .AppendHtml(content);
            }
        }
    }
}