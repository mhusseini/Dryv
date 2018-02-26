using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            var rules = RulesHelper.GetClientRulesForProperty(
                context.ModelMetadata.ContainerType,
                context.ModelMetadata.PropertyName);
            context.Attributes.Add("data-dryv", rules);
        }
    }
}