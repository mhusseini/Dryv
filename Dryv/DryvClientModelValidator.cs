using System;
using System.Collections.Generic;
using Dryv.Configuration;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public class DryvClientModelValidator : IDryvClientModelValidator
    {
        private const string DataValAttribute = "data-val";
        private const string DataValDryAttribute = "data-val-dryv";
        private readonly IOptions<DryvOptions> options;

        public DryvClientModelValidator(IOptions<DryvOptions> options)
        {
            this.options = options;
        }

        public virtual void AddValidation(ClientModelValidationContext context, IEnumerable<(string Path, DryvRule Rule)> rules)
        {
            var modelType = context.ModelMetadata.ContainerType;
            var modelPath = context.GetModelPath();
            var translatedRules = this.TranslateRules(context, rules, modelPath, modelType);
            var script = $@"[{string.Join(",", translatedRules)}]";

            context.Attributes.Add(DataValAttribute, "true");
            context.Attributes.Add(DataValDryAttribute, script);
        }

        protected IEnumerable<string> TranslateRules(ClientModelValidationContext context, IEnumerable<(string Path, DryvRule Rule)> rules, string modelPath, Type modelType)
        {
            return rules.Translate(
                context.ActionContext.HttpContext.RequestServices.GetService,
                this.options.Value,
                modelPath,
                modelType);
        }
    }
}