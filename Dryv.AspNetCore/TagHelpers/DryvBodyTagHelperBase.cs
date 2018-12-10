using Dryv.AspNetCore.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.AspNetCore.TagHelpers
{
    public abstract class DryvBodyTagHelperBase : TagHelper
    {
        private readonly IDryvScriptBlockGenerator scriptBlockGenerator;

        protected DryvBodyTagHelperBase(IDryvScriptBlockGenerator scriptBlockGenerator)
        {
            this.scriptBlockGenerator = scriptBlockGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected string GetContent()
        {
            var result = this.ViewContext.Load();
            return this.scriptBlockGenerator.GetScriptBody(result);
        }
    }
}