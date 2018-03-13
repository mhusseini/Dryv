using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dryv.DependencyInjection;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvRulesAttribute : ValidationAttribute, IClientModelValidator
    {
        private const string ClientAttributeName = "data-val-dryv";

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add(ClientAttributeName, GetClientRules(context));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
            GetValidationResult(GetFirstErrorMessage(validationContext));

        private static string GetClientRules(ModelValidationContextBase context)
        {
            var translator = context.ActionContext.HttpContext.RequestServices.GetService<ITranslator>();
            var options = context.ActionContext.HttpContext.RequestServices.GetService<IOptions<DryvOptions>>();
            var metadata = context.ModelMetadata;
            return RulesHelper.GetClientRulesForProperty(
                metadata.ContainerType,
                metadata.PropertyName,
                translator,
                options.Value);
        }

        private static IEnumerable<Func<object, DryvResult>> GetCompiledRules(ValidationContext validationContext) =>
            RulesHelper.GetCompiledRulesForProperty(
                validationContext.ObjectType,
                validationContext.MemberName);

        private static IEnumerable<string> GetErrorMessages(ValidationContext validationContext) =>
            from rule in GetCompiledRules(validationContext)
            let e = rule(validationContext.ObjectInstance)
            where e.IsError()
            select e.Message;

        private static string GetFirstErrorMessage(ValidationContext validationContext) =>
            GetErrorMessages(validationContext).FirstOrDefault();

        private static ValidationResult GetValidationResult(string errorMessage) =>
            errorMessage == null
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
    }
}