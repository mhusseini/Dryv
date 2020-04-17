using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.Internal
{
    internal class DryvModelValidator : IModelValidator
    {
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context.Container == null)
            {
                return Array.Empty<ModelValidationResult>();
            }

            var validator = context.ActionContext.HttpContext.RequestServices.GetService<DryvValidator>();
            var validationResults = this.GetGroupedValidationResults(context, validator);
            return CreateModelValidationResults(context, validationResults);
        }

        private static IEnumerable<ModelValidationResult> CreateModelValidationResults(ModelValidationContext context, IReadOnlyDictionary<object, Dictionary<string, List<DryvResult>>> validationResults)
            => !validationResults.TryGetValue(context.Container, out var d) || !d.TryGetValue(context.ModelMetadata.PropertyName, out var results)
            ? (IEnumerable<ModelValidationResult>)Array.Empty<ModelValidationResult>()
            : (from r in results
               from msg in r.Message
               where msg.IsError()
               select new ModelValidationResult(null, msg.Text)).ToList();

        private Dictionary<object, Dictionary<string, List<DryvResult>>> GetGroupedValidationResults(ModelValidationContext context, DryvValidator validator)
        {
            return context.ActionContext.HttpContext.GetDryvFeature()
                .CurrentValidationResults
                .GetOrAdd(context.ActionContext, _ => this.ValidateCore(context, validator)
                    .GroupBy(r => r.Model)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(i => i.Property.Name).ToDictionary(
                            i2 => i2.Key,
                            i2 => i2.ToList())));
        }

        private IEnumerable<DryvResult> ValidateCore(ModelValidationContext context, DryvValidator validator)
        {
            return validator.Validate(context.Container, context.ActionContext.HttpContext.RequestServices.GetService);
        }
    }
}