using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dryv
{
    internal class DryvModelValidator : IModelValidator
    {
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            var validationResults = GetGroupedValidationResults(context);
            return CreateModelValidationResults(context, validationResults);
        }

        private static IEnumerable<ModelValidationResult> CreateModelValidationResults(ModelValidationContext context, IReadOnlyDictionary<object, Dictionary<string, List<DryvValidationResult>>> validationResults)
            => !validationResults.TryGetValue(context.Container, out var d) || !d.TryGetValue(context.ModelMetadata.PropertyName, out var results)
            ? (IEnumerable<ModelValidationResult>)Array.Empty<ModelValidationResult>()
            : (from r in results
               from msg in r.Message
               where msg.IsError()
               select new ModelValidationResult(null, msg.Text)).ToList();

        private static Dictionary<object, Dictionary<string, List<DryvValidationResult>>> GetGroupedValidationResults(ModelValidationContext context)
            => context.ActionContext.HttpContext.GetDryvFeature()
            .CurrentValidationResults
            .GetOrAdd(context.ActionContext, _ => ValidateCore(context)
                .GroupBy(r => r.Model)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(i => i.Property.Name).ToDictionary(
                        i2 => i2.Key,
                        i2 => i2.ToList())));

        private static IEnumerable<DryvValidationResult> ValidateCore(ModelValidationContext context)
            => DryvValidator.Validate(context.Container, context.ActionContext.HttpContext.RequestServices.GetService);
    }
}