using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.Configuration;
using Dryv.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.TagHelpers
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
            var property = this.AspFor.GetProperty();
            var modelPath = this.ViewContext.GetModelPath(this.AspFor);
            var httpContext = this.ViewContext.HttpContext;

            var clientValidation = httpContext.RequestServices.GetService<IDryvClientModelValidator>().GetValidationAttributes(
                modelType,
                modelPath,
                property,
                this.ViewContext.HttpContext.RequestServices.GetService,
                this.options.Value);

            if (clientValidation == null)
            {
                return Task.CompletedTask;
            }

            this.ViewContext.StoreValidationCode(clientValidation.Name, clientValidation);
            output.Attributes.AddRange(clientValidation.ElementAttribute.Select(i => new TagHelperAttribute(i.Key, i.Value)));

            return Task.CompletedTask;
        }
    }
}