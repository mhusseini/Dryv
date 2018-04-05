﻿using System;
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
            var rules = from rule in RulesFinder.GetRulesForProperty(modelType, property)
                        where rule.IsEnabled(services.GetService) &&
                              rule.EvaluationLocation.HasFlag(RuleEvaluationLocation.Client)
                        select rule;

            var clientValidator = services.GetService<IDryvClientModelValidator>();
            clientValidator.AddValidation(context, rules);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var httpConextAccessor = context.GetService<IHttpContextAccessor>();
            var httpContext = httpConextAccessor.HttpContext;
            //var modelType = (context. as ViewContext)?.ViewData.Model?.GetType()
            //                ?? context.ObjectType;
            var property = context.GetProperty();
            var modelType = context.ObjectType;
            var errorMessage = (from rule in RulesFinder.GetRulesForProperty(modelType, property)
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