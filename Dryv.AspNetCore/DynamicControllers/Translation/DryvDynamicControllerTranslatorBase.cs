using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Translation;
using Dryv.Translation.Visitors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal abstract class DryvDynamicControllerTranslatorBase
    {
        private static readonly ConcurrentDictionary<string, Lazy<TypeInfo>> Controllers = new ConcurrentDictionary<string, Lazy<TypeInfo>>();
        private readonly ControllerGenerator codeGenerator;
        private readonly IDryvClientServerCallWriter controllerCallWriter;
        private readonly DryvDynamicControllerRegistration controllerRegistration;
        private readonly LinkGenerator linkGenerator;
        private readonly IOptions<MvcOptions> mvcOptions;
        private readonly IOptions<DryvDynamicControllerOptions> options;

        protected DryvDynamicControllerTranslatorBase(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, IOptions<MvcOptions> mvcOptions, LinkGenerator linkGenerator)
        {
            this.controllerRegistration = controllerRegistration;
            this.codeGenerator = codeGenerator;
            this.controllerCallWriter = controllerCallWriter;
            this.options = options;
            this.mvcOptions = mvcOptions;
            this.linkGenerator = linkGenerator;
        }

        protected void TranslateToServerCall(TranslationContext context, ITranslator translator, Expression expression, string action)
        {
            context.IsAsync = true;

            if (context.WhatIfMode)
            {
                return;
            }

            var controller = this.GenerateController(context, expression.ToString(), expression, action);
            var url = this.mvcOptions.Value.EnableEndpointRouting ? this.GetUrlFromEndpoint(controller) : GetUrlFromAttributes(controller);
            var httpMethod = this.options.Value.HttpMethod.ToString().ToUpper();
            var modelProperties = FindModelPropertiesInExpression(context, expression);

            this.controllerCallWriter.Write(context, translator, url, httpMethod, modelProperties);
        }

        private static List<MemberExpression> FindModelPropertiesInExpression(TranslationContext context, Expression expression)
        {
            var f = new ExpressionNodeFinder<MemberExpression>();

            f.Visit(expression);

            return f.FoundChildren
                .Where(e => e.Member is PropertyInfo)
                .Where(e => ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic(e).Any(p => p.Type == context.ModelType))
                .Distinct(MemberExpressionComparer.Default)
                .ToList();
        }

        private static string GetUrlFromAttributes(Type controller)
        {
            var routeAttribute = controller.GetMethods().Select(m => m.GetCustomAttribute<RouteAttribute>()).First(a => a != null);
            var url = routeAttribute.Template;

            if (!url.StartsWith("/"))
            {
                url = "/" + url;
            }

            return url;
        }

        private TypeInfo GenerateController(TranslationContext context, string key, Expression expression, string action)
        {
            return Controllers.GetOrAdd(key, _ => new Lazy<TypeInfo>(() =>
            {
                var assembly = this.codeGenerator.CreateControllerAssembly(expression, context.ModelType, action, context.Rule);
                this.controllerRegistration.Register(assembly, action);

                return assembly.DefinedTypes.FirstOrDefault(typeof(Controller).IsAssignableFrom);
            })).Value;
        }

        private string GetUrlFromEndpoint(Type controller)
        {
            return this.linkGenerator.GetPathByRouteValues(controller.Name, null);
        }
    }
}