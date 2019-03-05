using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public static class HtmlExtensions
    {
        public static IHtmlContent DryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"The parameter '{nameof(propertyExpression)}' must be a property.");
            }

            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();

            var modelType = htmlHelper.ViewData.Model.GetType();
            var modelPath = String.Empty;

            var clientValidation = services.GetService<IDryvClientValidationProvider>().GetClientPropertyValidation(
                modelType,
                modelPath,
                property,
                services.GetService,
                options.Value);

            return new HtmlString(clientValidation.ValidationFunction);
        }

        public static IEnumerable<DryvClientPropertyValidation> GetDryvClientValidation<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();

            var modelType = htmlHelper.ViewData.Model.GetType();
            var validator = services.GetService<IDryvClientValidationProvider>();

            return CollectClientValidation(modelType, null, null, validator, new HashSet<Type>(), options.Value, services.GetService).ToList();
        }


        internal static IEnumerable<DryvClientPropertyValidation> CollectClientValidation(Type modelType, Type rootModelType, string modelPath, IDryvClientValidationProvider validator, ISet<Type> processedTypes, DryvOptions options, Func<Type, object> services)
        {
            if (rootModelType == null)
            {
                rootModelType = modelType;
            }

            if (modelPath == null)
            {
                modelPath = string.Empty;
            }

            processedTypes.Add(modelType);
            var properties = modelType.GetProperties();

            foreach (var validation in from property in properties
                                       let clientValidation = validator.GetClientPropertyValidation(rootModelType, modelPath, property, services, options)
                                       where clientValidation != null
                                       select clientValidation)
            {
                yield return validation;
            }

            foreach (var validation in from p in properties
                                       where !p.PropertyType.IsValueType && !p.PropertyType.HasElementType && p.PropertyType != typeof(string) && !processedTypes.Contains(p.PropertyType)
                                       let prefix = string.IsNullOrWhiteSpace(modelPath) ? string.Empty : $"{modelPath}."
                                       from v in CollectClientValidation(p.PropertyType, rootModelType, $"{prefix}{p.Name.ToCamelCase()}", validator, processedTypes, options, services)
                                       select v)
            {
                yield return validation;
            }
        }
    }
}