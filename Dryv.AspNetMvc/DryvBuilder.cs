using Dryv.Configuration;
using Dryv.Translation;

namespace Dryv.AspNetMvc
{
    internal class DryvBuilder : IDryvBuilder
    {
        private readonly IDependencyContainer container;

        public DryvBuilder(IDependencyContainer container)
        {
            this.container = container;
        }

        public IDryvBuilder AddTranslator<T>()
        {
            var type = typeof(T);

            if (typeof(IMethodCallTranslator).IsAssignableFrom(type))
            {
                this.container.AddSingleton(typeof(IMethodCallTranslator), type);
            }

            if (typeof(ICustomTranslator).IsAssignableFrom(type))
            {
                this.container.AddSingleton(typeof(ICustomTranslator), type);
            }

            return this;
        }

        public IDryvBuilder AddTranslator(object translator)
        {
            if (translator is IMethodCallTranslator methodCallTranslator)
            {
                this.container.AddInstance(typeof(IMethodCallTranslator), methodCallTranslator);
            }

            if (translator is ICustomTranslator genericTranslator)
            {
                this.container.AddInstance(typeof(ICustomTranslator), genericTranslator);
            }

            return this;
        }
    }
}