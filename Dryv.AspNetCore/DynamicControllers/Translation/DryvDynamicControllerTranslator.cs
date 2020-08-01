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

            var expression = context.Expression;
            if (expression is LambdaExpression lambda)
            {
                return false;
            }

            var finder = new AsyncMethodCallFinder(this.methodCallTranslators, context);

            if (expression is ConditionalExpression conditional)
            {
                switch (finder.FindAsyncMethodCalls(conditional.Test).Count)
                {
                    case 0:
                        return false;
                    case 1:
                        {
                            var c = finder.FindAsyncMethodCalls(conditional.IfTrue).Count;
                            c += finder.FindAsyncMethodCalls(conditional.IfFalse).Count;

                            if (c == 0)
                            {
                                return false;
                            }

                            break;
                        }
                }
            }
            else if (finder.FindAsyncMethodCalls(expression).Count < 2)
            {
                return false;
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                var parameters = new[]
                {
                    lambdaExpression.Parameters
                        .Where(p => p.Type == context.ModelType)
                        .Select(p => context.Translator.FormatIdentifier(p.Name))
                        .FirstOrDefault(),
                    "$ctx"
                }.Where(p => !string.IsNullOrWhiteSpace(p));

                context.Writer.Write("function(");
                context.Writer.Write(string.Join(", ", parameters));
                context.Writer.Write(") {");
                context.Writer.Write("return ");
            }

            this.TranslateToServerCall(context, context.Translator, expression, "Process");

            if (expression.NodeType == ExpressionType.Lambda)
            {
                context.Writer.Write("}");
            }

            return true;
        }
    }
}