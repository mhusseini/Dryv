using Dryv.MethodCallTranslation;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.DependencyInjection
{
    public class DryvBuilder : IDryvBuilder
    {
        public DryvBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; }

        public IDryvBuilder AddTranslator<T>()
        {
            var type = typeof(T);

            if (typeof(IMethodCallTranslator).IsAssignableFrom(type))
            {
                this.Services.AddSingleton(typeof(IMethodCallTranslator), type);
            }

            if (typeof(IGenericTranslator).IsAssignableFrom(type))
            {
                this.Services.AddSingleton(typeof(IGenericTranslator), type);
            }

            return this;
        }

        public IDryvBuilder AddTranslator(object translator)
        {
            if (translator is IMethodCallTranslator methodCallTranslator)
            {
                this.Services.AddSingleton(methodCallTranslator);
            }

            if (translator is IGenericTranslator genericTranslator)
            {
                this.Services.AddSingleton(genericTranslator);
            }

            return this;
        }
    }
}