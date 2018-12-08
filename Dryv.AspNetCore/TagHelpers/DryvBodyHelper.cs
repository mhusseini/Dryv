using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

            this.ViewContext.TempData.Remove(ViewContextExtensions.ItemsKey);
        }
    }

    public class DryvBodyTagHelperBase : TagHelper
    {
        private readonly IDryvScriptBlockGenerator scriptBlockGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public DryvBodyTagHelperBase(IDryvScriptBlockGenerator scriptBlockGenerator)
        {
            this.scriptBlockGenerator = scriptBlockGenerator;
        }

        protected string GetContent()
        {
            var result = this.ViewContext.Load();
            return this.scriptBlockGenerator.GetScriptBody(result);
        }
    }

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