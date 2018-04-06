using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dryv.Compilation;
using Dryv.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvRulesAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            var modelType = (context.ActionContext as ViewContext)?.ViewData.ModelMetadata.ModelType
                            ?? context.ModelMetadata.ContainerType;
            var services = context.ActionContext.HttpContext.RequestServices;
            var property = context.GetProperty();
            var modelPath = context.GetModelPath();
            var rules = from rule in modelType.GetRulesForProperty(property, modelPath)
                        where rule.Rule.IsEnabled(services.GetService) &&
                              rule.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Client)
                        select rule;

            services.GetService<IDryvClientModelValidator>().AddValidation(context, rules);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var httpConextAccessor = context.GetService<IHttpContextAccessor>();
            var httpContext = httpConextAccessor.HttpContext;
            //var modelType = (context. as ViewContext)?.ViewData.Model?.GetType()
            //                ?? context.ObjectType;
            var property = context.GetProperty();
            var modelType = context.ObjectType;
            var errorMessage = (from rule in modelType.GetRulesForProperty(property, null)
                                where rule.Rule.IsEnabled(context.GetService) &&
                                      rule.Rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Server)
                                let result = rule.Rule.Validate(context.ObjectInstance, context.GetService)
                                where result.IsError()
                                select result.Message).FirstOrDefault();

            return errorMessage == null
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}