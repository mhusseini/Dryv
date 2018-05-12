using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Dryv.Configuration;
using Dryv.Utils;

namespace Dryv.AspNetMvc
{
    internal class DryvModelValidator : ModelValidator
    {
        public DryvModelValidator(ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var modelType = this.ControllerContext.Controller.ViewData.ModelMetadata?.ModelType
                            ?? this.Metadata.Container?.GetType()
                            ?? this.Metadata.ContainerType;
            var services = DependencyResolver.Current;
            var property = this.Metadata.ContainerType.GetRuntimeProperty(this.Metadata.PropertyName);
            var options = services.GetService(typeof(DryvOptions)) as DryvOptions;
            var modelPath = this.GetModelPath(
                this.ControllerContext.Controller.ViewData.ModelMetadata ?? this.Metadata,
                this.Metadata.ContainerType,
                this.Metadata.PropertyName);

            var attributes = services.GetService<IDryvClientModelValidator>().GetValidationAttributes(
                modelType,
                modelPath,
                property,
                services.GetService,
                options);

            return new[]
            {
                new DryvModelClientValidationRule(attributes)
            };
        }

        private string GetModelPath(ModelMetadata metadata, Type modelType, string propertyName)
        {
            var propertyPath = this.FindProperty(
                string.Empty,
                metadata,
                modelType,
                propertyName);

            if (propertyPath?.Length == 0)
            {
                propertyPath = null;
            }

            if (propertyPath != null)
            {
                return string.Join(".", propertyPath
                    .Split('.')
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .Select(n => n.ToCamelCase()));
            }

            return null;
        }

        private string FindProperty(string path, ModelMetadata metadata, Type modelType, string propertyName)
        {
            return metadata.ContainerType == modelType && metadata.PropertyName == propertyName
                ? path
                : metadata.Properties
                    .Select(child => this.FindProperty(
                        path + (string.IsNullOrWhiteSpace(path) ? string.Empty : ".") + child.PropertyName,
                        child,
                        modelType,
                        propertyName))
                    .FirstOrDefault(r => r != null);
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            yield break;
        }
    }
}