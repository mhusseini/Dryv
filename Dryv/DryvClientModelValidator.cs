using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Utils;
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
            context.Attributes.TryGetValue("name", out string modelName);
            if (modelName?.Length == 0)
            {
                modelName = null;
            }

            if (modelName != null)
            {
                modelName = string.Join(".", modelName
                    .Split(".")
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .Select(n => n.ToCamelCase()));
            }
            var translatedRules = this.TranslateRules(context, rules, this.options, modelName);
            var script = $@"[{string.Join(",", translatedRules)}]";

            context.Attributes.Add(DataValAttribute, "true");
            context.Attributes.Add(DataValDryAttribute, script);
        }

        protected IEnumerable<string> TranslateRules(ClientModelValidationContext context, IEnumerable<DryvRule> rules, IOptions<DryvOptions> options, string modelName)
        {
            var services = context.ActionContext.HttpContext.RequestServices;
            var translatedRules = rules.Translate(services.GetService, options.Value, modelName);
            return translatedRules;
        }
    }
}