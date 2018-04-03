using Dryv.Translation;
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

            if (typeof(ICustomTranslator).IsAssignableFrom(type))
            {
                this.Services.AddSingleton(typeof(ICustomTranslator), type);
            }

            return this;
        }

        public IDryvBuilder AddTranslator(object translator)
        {
            if (translator is IMethodCallTranslator methodCallTranslator)
            {
                this.Services.AddSingleton(methodCallTranslator);
            }

            if (translator is ICustomTranslator genericTranslator)
            {
                this.Services.AddSingleton(genericTranslator);
            }

            return this;
        }
    }
}