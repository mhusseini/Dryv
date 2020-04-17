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
        private static readonly HashSet<string> RestrictedBaseTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.AspNetCore.Mvc.RazorPages.PageModel"
        };

        public static IHtmlContent DryvClientValidationFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression) || !(memberExpression.Member is PropertyInfo property))
            {
                throw new ArgumentException($"The parameter '{nameof(propertyExpression)}' must be a property.");
            }

            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();

            var modelType = htmlHelper.ViewData.Model.GetType();
            var modelPath = string.Empty;

            var clientValidation = services.GetService<IDryvClientValidationProvider>().GetClientPropertyValidation(
                modelType,
                modelPath,
                property,
                services.GetService,
                options.Value);

            return new HtmlString(clientValidation.ValidationFunction);
        }

        public static IEnumerable<DryvClientValidationNamedGroup> GetDryvClientGroupValidations<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var validationItems = EnumerateValidationItems(htmlHelper);

            var l = from g in
                        from v in validationItems
                        where !string.IsNullOrWhiteSpace(v.GroupName)
                        group v by v.GroupName
                    where g.Count() > 1
                    let first = g.First()
                    select new DryvClientValidationNamedGroup
                    {
                        Properties = g.Select(x => x.Property).ToList(),
                        GroupName = first.GroupName,
                        Key = first.Key,
                        ValidationFunction = first.ValidationFunction,
                        ModelPath = first.ModelPath,
                        ModelType = first.ModelType
                    };

            return l.ToList();
        }

        public static IEnumerable<DryvClientValidationItem> GetDryvClientPropertyValidations<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            var l = from i in EnumerateValidationItems(htmlHelper)
                    where i.GroupName == null
                    select i;

            return l.ToList();
        }

        internal static IEnumerable<DryvClientValidationItem> CollectClientValidation(Type modelType, Type rootModelType, string modelPath, IDryvClientValidationProvider validator, ISet<string> processedTypes, DryvOptions options, Func<Type, object> services)
        {
            if (rootModelType == null)
            {
                rootModelType = modelType;
            }

            if (modelPath == null)
            {
                modelPath = string.Empty;
            }

            processedTypes.Add(modelPath);
            var properties = (from p in modelType.GetProperties()
                              where p.DeclaringType != null && !RestrictedBaseTypes.Contains(p.DeclaringType.FullName)
                              select p).ToList();

            foreach (var validation in GetPropertyValidations(rootModelType, modelPath, validator, options, services, properties))
            {
                yield return validation;
            }

            foreach (var validation in GetChildValidations(rootModelType, modelPath, validator, options, services, properties, processedTypes))
            {
                yield return validation;
            }
        }

        private static IEnumerable<DryvClientValidationItem> EnumerateValidationItems<TModel>(IHtmlHelper<TModel> htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            var options = services.GetService<IOptions<DryvOptions>>();

            var modelType = htmlHelper.ViewData.Model.GetType();
            var validator = services.GetService<IDryvClientValidationProvider>();

            return from i in CollectClientValidation(modelType, null, null, validator, new HashSet<string>(), options.Value, services.GetService)
                   where i != null
                   select i;
        }

        private static IEnumerable<DryvClientValidationItem> GetChildValidations(Type rootModelType, string modelPath, IDryvClientValidationProvider validator, DryvOptions options, Func<Type, object> services, IEnumerable<PropertyInfo> properties, ISet<string> processedTypes)
        {
            return from p in properties
                   let prefix = string.IsNullOrWhiteSpace(modelPath) ? string.Empty : $"{modelPath}."
                   let childPath = $"{prefix}{p.Name.ToCamelCase()}"
                   where !p.PropertyType.IsValueType && !p.PropertyType.HasElementType && p.PropertyType != typeof(string) && !processedTypes.Contains(childPath)
                   from v in CollectClientValidation(p.PropertyType, rootModelType, childPath, validator, processedTypes, options, services)
                   select v;
        }

        //private static IEnumerable<DryvClientValidationItem> GetGroupValidations(Type rootModelType, string modelPath, IDryvClientValidationProvider validator, DryvOptions options, Func<Type, object> services, IEnumerable<PropertyInfo> properties)
        //{
        //    return from property in properties
        //           from clientValidation in validator.GetClientPropertyValidationGroups(rootModelType, modelPath, property, services, options)
        //           select clientValidation;
        //}

        private static IEnumerable<DryvClientValidationItem> GetPropertyValidations(Type rootModelType, string modelPath, IDryvClientValidationProvider validator, DryvOptions options, Func<Type, object> services, IEnumerable<PropertyInfo> properties)
        {
            return from property in properties
                   select validator.GetClientPropertyValidation(rootModelType, modelPath, property, services, options);
        }
    }
}