using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvRulesAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string DataValAttribute = "data-val";
        private const string DataValDryAttribute = "data-val-dryv";

        public void AddValidation(ClientModelValidationContext context)
        {
            var services = context.ActionContext.HttpContext.RequestServices;
            var property = context.GetProperty();
            var rules = from rule in RulesFinder.GetRulesForProperty(property)
                        where rule.IsEnabled(services.GetService)
                        select rule;
            var translatedRules = rules.Translate(services.GetService);

            context.Attributes.Add(DataValAttribute, "true");
            context.Attributes.Add(DataValDryAttribute, translatedRules);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var property = context.GetProperty();
            var errorMessage = (from rule in RulesFinder.GetRulesForProperty(property)
                                where rule.IsEnabled(context.GetService)
                                let result = rule.Validate(context.ObjectInstance, context.GetService)
                                where result.IsError()
                                select result.Message).FirstOrDefault();

            return errorMessage == null
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}