using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dryv.Utils;

namespace Dryv
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DryvRulesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = context.ObjectInstance;
            var modelProvider = context.GetService<IModelProvider>() ?? AddModelProvider(context, model);

            var errorMessage = DryvValidator
                .ValidateProperty(
                    model,
                    modelProvider.GetModel(),
                    context.GetProperty(),
                    context.GetService,
                    context.Items)
                .Where(i => i.IsError())
                .Select(i => i.Message)
                .FirstOrDefault();

            return errorMessage == null
                ? ValidationResult.Success
                : new ValidationResult(errorMessage, new[] { context.MemberName });
        }

        private static IModelProvider AddModelProvider(ValidationContext context, object model)
        {
            Func<Type, object> serviceProvider = context.GetService;
            var modelProvider = new ModelProvider(model);
            context.InitializeServiceProvider(t => t == typeof(IModelProvider)
                ? modelProvider
                : serviceProvider(t));
            return modelProvider;
        }

        private class ModelProvider : IModelProvider
        {
            private readonly object model;

            public ModelProvider(object model) => this.model = model;

            public object GetModel() => this.model;
        }
    }
}