using Dryv.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.TagHelpers
{
    public class DryvBodyTagHelperBase : TagHelper
    {
        private readonly IDryvScriptBlockGenerator scriptBlockGenerator;

        public DryvBodyTagHelperBase(IDryvScriptBlockGenerator scriptBlockGenerator)
        {
            this.scriptBlockGenerator = scriptBlockGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected virtual string GetContent()
        {
            var result = this.ViewContext.PopValidationCode();
            return this.scriptBlockGenerator.GetScriptBody(result);
        }
    }
}