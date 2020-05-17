using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dryv.AspNetCore.DynamicControllers.CodeGeneration;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerTranslator : ICustomTranslator
    {
        private static readonly ConcurrentDictionary<string, Lazy<TypeInfo>> Controllers = new ConcurrentDictionary<string, Lazy<TypeInfo>>();
        private readonly ControllerGenerator codeGenerator;
        private readonly IDryvClientServerCallWriter controllerCallWriter;
        private readonly DryvDynamicControllerRegistration controllerRegistration;
        private readonly IOptions<DryvDynamicControllerOptions> options;
        private readonly LinkGenerator linkGenerator;

        public DryvDynamicControllerTranslator(DryvDynamicControllerRegistration controllerRegistration, ControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options, LinkGenerator linkGenerator)
        {
            this.controllerRegistration = controllerRegistration;
            this.codeGenerator = codeGenerator;
            this.controllerCallWriter = controllerCallWriter;
            this.options = options;
            this.linkGenerator = linkGenerator;
        }

        public bool? AllowSurroundingBrackets(Expression expression)
        {
            return true;
        }

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MethodCallExpression methodCallExpression))
            {
                return false;
            }

            if (!typeof(Task).IsAssignableFrom(methodCallExpression.Method.ReturnType))
            {
                return false;
            }

            var controller = this.GenerateController(methodCallExpression);

            var method = methodCallExpression.Method;
            var methodParameters = method.GetParameters();
            var parameters = methodCallExpression.Arguments
                .Select((e, i) => (e, i))
                .ToDictionary(x => methodParameters[x.i], x => x.e);
            var route = controller.FullName;

            var url = this.linkGenerator.GetPathByRouteValues(controller.Name, null);
            var httpMethod = this.options.Value.HttpMethod.ToString().ToUpper();

            this.controllerCallWriter.Write(context, url, httpMethod, parameters);

            return true;
        }

        private TypeInfo GenerateController(MethodCallExpression methodCallExpression)
        {
            var m = methodCallExpression.Method;
            var key = $"{m.DeclaringType?.FullName}|{m.Name}|{string.Join("|", m.GetParameters().Select(p => p.ParameterType.FullName))}";

            return Controllers.GetOrAdd(key, _ => new Lazy<TypeInfo>(() =>
            {
                var assembly = this.codeGenerator.CreateControllerAssembly(methodCallExpression);
                this.controllerRegistration.Register(assembly, methodCallExpression.Method);

                return assembly.DefinedTypes.FirstOrDefault(typeof(Controller).IsAssignableFrom);
            })).Value;
        }
    }
}