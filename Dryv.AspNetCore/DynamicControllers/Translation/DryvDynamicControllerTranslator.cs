using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Translation;
using Dryv.Translation.Visitors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerTranslator : IDryvMethodCallTranslator, IDryvCustomTranslator
    {
        private static readonly ConcurrentDictionary<string, Lazy<TypeInfo>> Controllers = new ConcurrentDictionary<string, Lazy<TypeInfo>>();
        private readonly ControllerGenerator codeGenerator;
        private readonly IDryvClientServerCallWriter controllerCallWriter;
        private readonly DryvDynamicControllerRegistration controllerRegistration;
        private readonly LinkGenerator linkGenerator;
        private readonly IOptions<MvcOptions> mvcOptions;
        private readonly IOptions<DryvDynamicControllerOptions> options;
        private readonly TranslatorProvider translatorProvider;

        public DryvDynamicControllerTranslator(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, IOptions<MvcOptions> mvcOptions, LinkGenerator linkGenerator, TranslatorProvider translatorProvider)
        {
            this.controllerRegistration = controllerRegistration;
            this.codeGenerator = codeGenerator;
            this.controllerCallWriter = controllerCallWriter;
            this.options = options;
            this.mvcOptions = mvcOptions;
            this.linkGenerator = linkGenerator;
            this.translatorProvider = translatorProvider;
        }

        public int? OrderIndex { get; set; } = int.MaxValue;
        public bool? AllowSurroundingBrackets(Expression expression)
        {
            return false;
        }

        public bool TryTranslate(CustomTranslationContext context)
        {
            //if (!this.options.Value.Greedy)
            //{
            //    return false;
            //}

            const string key = nameof(DryvDynamicControllerTranslator);
            
            if (context.CustomData.ContainsKey(key))
            {
                return false;
            }

            var finder = new AsyncMethodCallFinder(this.translatorProvider, context);
            finder.FindAsyncMethodCalls(context.Expression);

            if (finder.AsyncMethodCallsExpressions.Count < 2)
            {
                return false;
            }

            if (context.Expression is LambdaExpression lambdaExpression)
            {
                var parameters = new[]
                {
                        lambdaExpression.Parameters
                            .Where(p => p.Type == context.ModelType)
                            .Select(p => context.Translator.FormatIdentifier(p.Name))
                            .FirstOrDefault(),
                        "$context"
                    }.Where(p => !string.IsNullOrWhiteSpace(p));

                context.Writer.Write("function(");
                context.Writer.Write(string.Join(", ", parameters));
                context.Writer.Write(") {");
                context.Writer.Write("return ");
            }

            this.TranslateToServerCall(context, context.Translator, context.Expression, "Process");

            if (context.Expression.NodeType == ExpressionType.Lambda)
            {
                context.Writer.Write("}");
            }

            return true;
        }

        public bool SupportsType(Type type)
        {
            return true;
        }

        public bool Translate(MethodTranslationContext context)
        {
            context.IsAsync = true;

            if (context.WhatIfMode)
            {
                return true;
            }

            var methodCallExpression = context.Expression;
            var isAsync = typeof(Task).IsAssignableFrom(methodCallExpression.Method.DeclaringType);

            if (isAsync && methodCallExpression.Method.Name == nameof(Task.FromResult))
            {
                context.Translator.Translate(methodCallExpression.Arguments.First(), context);
            }
            else
            {
                this.TranslateToServerCall(context, context.Translator, methodCallExpression, methodCallExpression.Method.Name);
            }

            return true;
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

        private void TranslateToServerCall(TranslationContext context, ITranslator translator, Expression expression, string action)
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
    }
}