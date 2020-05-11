using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore
{
    public class DryvClientValidationLoader
    {
        private readonly IDryvClientValidationProvider clientValidationProvider;
        private readonly IOptions<DryvOptions> options;
        private readonly IServiceProvider serviceProvider;

        public DryvClientValidationLoader(IDryvClientValidationProvider clientValidationProvider, IOptions<DryvOptions> options, IServiceProvider serviceProvider)
        {
            this.clientValidationProvider = clientValidationProvider;
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public DryvClientValidationItem GetDryvClientValidation<TModel>(Expression<Func<TModel, object>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"The parameter '{nameof(propertyExpression)}' must be a property.");
            }

            return this.clientValidationProvider.GetClientPropertyValidation(typeof(TModel), string.Empty, property, this.serviceProvider.GetService, this.options.Value);
        }

        public DryvClientValidationItem GetDryvClientValidation(Type modelType, PropertyInfo property)
        {
            return this.clientValidationProvider.GetClientPropertyValidation(modelType, string.Empty, property, this.serviceProvider.GetService, this.options.Value);
        }

        public IList<DryvClientValidationItem> GetDryvClientValidation<TModel>()
        {
            return this.GetDryvClientValidation(typeof(TModel));
        }

        public IList<DryvClientValidationItem> GetDryvClientValidation(Type modelType)
        {
            return this.CollectClientValidation(modelType, modelType, string.Empty, new Stack<string>());
        }

        internal IList<DryvClientValidationItem> CollectClientValidation(Type modelType, Type rootModelType, string modelPath, Stack<string> processedTypes)
        {
            if (modelType == typeof(Type) ||
                modelType.Namespace?.StartsWith("System.Reflection") == true ||
                modelType.FullName == null ||
                processedTypes.Contains(modelType.FullName))
            {
                return new DryvClientValidationItem[0];
            }

            var properties = modelType.GetProperties();

            var items = from property in properties
                        let item = this.clientValidationProvider.GetClientPropertyValidation(modelType, modelPath, property, this.serviceProvider.GetService, this.options.Value)
                        where item != null
                        select item;

            processedTypes.Push(modelType.FullName);

            var childItems = from property in properties
                             let prefix = string.IsNullOrWhiteSpace(modelPath) ? string.Empty : $"{modelPath}."
                             let childPath = $"{prefix}{property.Name.ToCamelCase()}"
                             where !property.PropertyType.IsValueType && !property.PropertyType.HasElementType && property.PropertyType != typeof(string) && !processedTypes.Contains(childPath)
                             from item in CollectClientValidation(property.PropertyType, rootModelType, childPath, processedTypes)
                             select item;

            processedTypes.Pop();

            return items.Union(childItems).ToList();
        }
    }
}