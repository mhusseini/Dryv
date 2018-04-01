using System.Collections.Generic;
using Dryv.Configuration;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public class DryvClientModelValidator : IDryvClientModelValidator
    {
        private readonly IOptions<DryvOptions> options;
        private const string DataValAttribute = "data-val";
        private const string DataValDryAttribute = "data-val-dryv";

        public DryvClientModelValidator(IOptions<DryvOptions> options)
        {
            this.options = options;
        }

        public virtual void AddValidation(ClientModelValidationContext context, IEnumerable<DryvRule> rules)
        {
            var translatedRules = this.TranslateRules(context, rules, this.options);
            var script = $@"[{string.Join(",", translatedRules)}]";

            context.Attributes.Add(DataValAttribute, "true");
            context.Attributes.Add(DataValDryAttribute, script);
        }

        protected IEnumerable<string> TranslateRules(ClientModelValidationContext context, IEnumerable<DryvRule> rules, IOptions<DryvOptions> options)
        {
            var services = context.ActionContext.HttpContext.RequestServices;
            var translatedRules = rules.Translate(services.GetService, options.Value);
            return translatedRules;
        }
    }
}