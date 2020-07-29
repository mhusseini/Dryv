using System;
using System.Linq;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerMethodTranslator : DryvDynamicControllerTranslatorBase, IDryvMethodCallTranslator
    {

        public bool SupportsType(Type type)
        {
            return true;
        }
        public DryvDynamicControllerMethodTranslator(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, IOptions<MvcOptions> mvcOptions, LinkGenerator linkGenerator)
            : base(controllerRegistration, codeGenerator, controllerCallWriter, options, mvcOptions, linkGenerator) { }

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
    }
}