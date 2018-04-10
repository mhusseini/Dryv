using System.Collections.Generic;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public class DryvClientModelValidator : IDryvClientModelValidator
    {
        protected const string DataTypeDryAttribute = "data-type-dryv";
        protected const string DataValAttribute = "data-val";
        protected const string DataValDryAttribute = "data-val-dryv";
        private readonly IOptions<DryvOptions> options;

        public DryvClientModelValidator(IOptions<DryvOptions> options)
        {
            this.options = options;
        }

        protected DryvOptions Options => this.options.Value;

        public virtual void AddValidation(ClientModelValidationContext context, PropertyInfo property, IEnumerable<DryvRuleNode> rules)
        {
            var translatedRules = this.TranslateRules(context, rules);
            var script = $@"[{string.Join(",", translatedRules)}]";

            context.Attributes.Add(DataValAttribute, "true");
            context.Attributes.Add(DataValDryAttribute, script);
            context.Attributes.Add(DataTypeDryAttribute, property.PropertyType.GetJavaScriptType());
        }

        protected IEnumerable<string> TranslateRules(ClientModelValidationContext context, IEnumerable<DryvRuleNode> rules)
        {
            return rules.Translate(
                context.ActionContext.HttpContext.RequestServices.GetService,
                this.Options,
                context.GetModelPath(),
                context.ModelMetadata.ContainerType);
        }
    }
}