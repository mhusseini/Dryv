using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Translation;
using Dryv.Translation.Visitors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerTranslator : DryvDynamicControllerTranslatorBase, IDryvCustomTranslator
    {
        private readonly IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators;

        public DryvDynamicControllerTranslator(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, IOptions<MvcOptions> mvcOptions, LinkGenerator linkGenerator, IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators)
        : base(controllerRegistration, codeGenerator, controllerCallWriter, options, mvcOptions, linkGenerator)
        {
            this.methodCallTranslators = methodCallTranslators;
        }

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

            var finder = new AsyncMethodCallFinder(this.methodCallTranslators, context);
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
    }
}