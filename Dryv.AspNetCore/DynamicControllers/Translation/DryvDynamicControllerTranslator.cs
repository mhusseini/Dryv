using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Extensions;
using Dryv.Translation;
using Dryv.Translation.Visitors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerTranslator : IMethodCallTranslator
    {
        private static readonly ConcurrentDictionary<string, Lazy<TypeInfo>> Controllers = new ConcurrentDictionary<string, Lazy<TypeInfo>>();
        private readonly ControllerGenerator codeGenerator;
        private readonly IDryvClientServerCallWriter controllerCallWriter;
        private readonly DryvDynamicControllerRegistration controllerRegistration;
        private readonly IOptions<DryvDynamicControllerOptions> options;
        private readonly IOptions<MvcOptions> mvcOptions;

        public DryvDynamicControllerTranslator(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, IOptions<MvcOptions> mvcOptions)
        {
            this.controllerRegistration = controllerRegistration;
            this.codeGenerator = codeGenerator;
            this.controllerCallWriter = controllerCallWriter;
            this.options = options;
            this.mvcOptions = mvcOptions;
        }

        public int? OrderIndex { get; set; } = int.MaxValue;

        public bool SupportsType(Type type)
        {
            return true;
        }

        public bool Translate(MethodTranslationContext context)
        {
            return this.Translate(context, context.Translator, context.Expression);
        }

        private static List<MemberExpression> FindModelPropertiesInExpression(TranslationContext context, MethodCallExpression methodCallExpression)
        {
            var f = new ExpressionNodeFinder<MemberExpression>();

            f.Visit(methodCallExpression);

            return f.FoundChildren
                .Where(e => e.Member is PropertyInfo)
                .Where(e => e.Member.DeclaringType == context.ModelType)
                .ToList();
        }

        private TypeInfo GenerateController(MethodCallExpression methodCallExpression, TranslationContext context, List<MemberExpression> modelProperties)
        {
            var modelFields = string.Join("|", modelProperties.Select(p => p.Member.Name));
            var m = methodCallExpression.Method;
            var parameters = string.Join("|", m.GetParameters().Select(p => p.ParameterType.FullName));
            var key = $"{m.DeclaringType?.FullName}|{m.Name}|{parameters}|{modelFields}";

            return Controllers.GetOrAdd(key, _ => new Lazy<TypeInfo>(() =>
            {
                var assembly = this.codeGenerator.CreateControllerAssembly(methodCallExpression, context.ModelType);
                this.controllerRegistration.Register(assembly, methodCallExpression.Method);

                return assembly.DefinedTypes.FirstOrDefault(typeof(Controller).IsAssignableFrom);
            })).Value;
        }

        private bool Translate(TranslationContext context, ITranslator translator, Expression expression)
        {
            if (!(expression is MethodCallExpression methodCallExpression))
            {
                return false;
            }

            if (typeof(Task).IsAssignableFrom(methodCallExpression.Method.DeclaringType) && methodCallExpression.Method.Name == nameof(Task.FromResult))
            {
                if (context.WhatIfMode) return true;
                translator.Translate(methodCallExpression.Arguments.First(), context);
                return true;
            }

            context.IsAsync = true;
            if (context.WhatIfMode) return true;

            var modelProperties = FindModelPropertiesInExpression(context, methodCallExpression);
            var controller = this.GenerateController(methodCallExpression, context, modelProperties);
            string url;

            if (this.mvcOptions.Value.EnableEndpointRouting)
            {
                var linkGenerator = context.ServiceProvider.GetService<LinkGenerator>();
                url = linkGenerator.GetPathByRouteValues(controller.Name, null);
            }
            else
            {
                var actionContext = context.ServiceProvider.GetService<IActionContextAccessor>().ActionContext;
                var urlHelperFactory = context.ServiceProvider.GetService<IUrlHelperFactory>();
                var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
                url = urlHelper.Action(methodCallExpression.Method.Name, controller.Name.Replace("Controller", string.Empty));
            }
            
            var httpMethod = this.options.Value.HttpMethod.ToString().ToUpper();

            this.controllerCallWriter.Write(context, translator, url, httpMethod, modelProperties);

            return true;
        }
    }
}