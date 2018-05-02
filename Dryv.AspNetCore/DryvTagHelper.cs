using System.Linq;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    [HtmlTargetElement("textarea", Attributes = "asp-for")]
    public class DryvTagHelper : TagHelper
    {
        private readonly IOptions<DryvOptions> options;

        public DryvTagHelper(IOptions<DryvOptions> options)
        {
            this.options = options;
        }

        public ModelExpression AspFor { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (this.AspFor == null || this.ViewContext == null)
            {
                return Task.CompletedTask;
            }

            var modelType = this.ViewContext.ViewData.ModelMetadata.ModelType
                            ?? this.AspFor.Metadata.ContainerType;
            var services = this.ViewContext.HttpContext.RequestServices;
            var property = this.AspFor.GetProperty();
            var modelPath = this.ViewContext.GetModelPath(this.AspFor);

            var attributes = services.GetService<IDryvClientModelValidator>().GetValidationAttributes(
                modelType,
                modelPath,
                property,
                services.GetService,
                this.options.Value);

            output.Attributes.AddRange(attributes.Select(i => new TagHelperAttribute(i.Key, i.Value)));

            return Task.CompletedTask;
        }
    }
}