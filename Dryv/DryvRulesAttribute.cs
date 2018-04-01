using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvRulesAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            var services = context.ActionContext.HttpContext.RequestServices;
            var property = context.GetProperty();
            var rules = from rule in RulesFinder.GetRulesForProperty(property)
                        where rule.IsEnabled(services.GetService) &&
                              rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Client)
                        select rule;

            var clientValidator = services.GetService<IDryvClientModelValidator>();
            clientValidator.AddValidation(context, rules);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var property = context.GetProperty();
            var errorMessage = (from rule in RulesFinder.GetRulesForProperty(property)
                                where rule.IsEnabled(context.GetService) &&
                                      rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                                let result = rule.Validate(context.ObjectInstance, context.GetService)
                                where result.IsError()
                                select result.Message).FirstOrDefault();

            return errorMessage == null
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}