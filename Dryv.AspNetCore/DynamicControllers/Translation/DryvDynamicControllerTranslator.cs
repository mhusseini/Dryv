using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class DryvDynamicControllerTranslator : ICustomTranslator
    {
        private readonly DryvDynamicDelegatingControllerGenerator codeGenerator;
        private readonly IDryvClientServerCallWriter controllerCallWriter;
        private readonly IOptions<DryvDynamicControllerOptions> options;
        private readonly DryvDynamicControllerRegistration controllerRegistration;

        public DryvDynamicControllerTranslator(DryvDynamicControllerRegistration controllerRegistration, DryvDynamicDelegatingControllerGenerator codeGenerator, IDryvClientServerCallWriter controllerCallWriter, IOptions<DryvDynamicControllerOptions> options)
        {
            this.controllerRegistration = controllerRegistration;
            this.codeGenerator = codeGenerator;
            this.controllerCallWriter = controllerCallWriter;
            this.options = options;
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

            var ass = this.codeGenerator.CreateControllerAssembly(methodCallExpression);
            this.controllerRegistration.Register(ass);
            var c = ass.DefinedTypes.FirstOrDefault(t => typeof(Controller).IsAssignableFrom(t));

            var method = methodCallExpression.Method;
            var methodParameters = method.GetParameters();
            var parameters = methodCallExpression.Arguments
                .Select((e, i) => (e, i))
                .ToDictionary(x => methodParameters[x.i], x => x.e);
            var route = c.FullName;

            context.ClientCodeModifiers.Add(typeof(DryvDynamicControllerClientCodeModifier));
            var urlPlaceHolder = $"##route:{route}##";
            var httpMethod = this.options.Value.HttpMethod.ToString().ToUpper();

            this.controllerCallWriter.Write(context, urlPlaceHolder, httpMethod, parameters);

            return true;
        }
    }
}