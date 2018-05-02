using System;
using System.Web.Mvc;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Translation.Translators;

namespace Dryv.AspNetMvc
{
    public static class DryvMvc
    {
        public static IDryvBuilder Configure(IDependencyContainer container, Action<DryvOptions> setupAction = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            ModelValidatorProviders.Providers.Add(new DryModelValidatorProvider());

            var options = new DryvOptions();

            setupAction?.Invoke(options);

            container.RegisterSingleton(typeof(IDryvClientModelValidator), typeof(DryvClientModelValidator));
            container.RegisterSingleton(typeof(ITranslator), typeof(JavaScriptTranslator));
            container.RegisterSingleton(typeof(ITranslatorProvider), typeof(TranslatorProvider));
            container.RegisterInstance(typeof(DryvOptions), options);

            return new DryvBuilder(container)
                .AddTranslator<ObjectTranslator>()
                .AddTranslator<DryvResultTranslator>()
                .AddTranslator<StringTranslator>()
                .AddTranslator<EnumerableTranslator>()
                .AddTranslator<RegexTranslator>();
        }
    }
}