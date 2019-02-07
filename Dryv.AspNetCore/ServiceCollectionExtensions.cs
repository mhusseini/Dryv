using System;
using Dryv.Configuration;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public static class DryvMvcExtensions
    {
        public static IDryvBuilder AddDryv(this IMvcBuilder mvcBuilder, Action<DryvOptions> setupAction = null)
        {
            mvcBuilder.AddMvcOptions(options =>
            {
                options.ModelValidatorProviders.Add(new DryvModelValidatorProvider());

            });
            return RegsterServices(mvcBuilder.Services, setupAction);
        }

        private static IDryvBuilder RegsterServices(this IServiceCollection services, Action<DryvOptions> setupAction)
        {
            var options = new DryvOptions();

            setupAction?.Invoke(options);

            services.TryAddSingleton(typeof(IDryvClientModelValidator), options.ClientValidatorType ?? typeof(DryvClientModelValidator));
            services.TryAddSingleton(typeof(IDryvScriptBlockGenerator), options.ClientBodyGeneratorType ?? typeof(DryvScriptBlockGenerator));
            services.AddSingleton<ITranslator, JavaScriptTranslator>();
            services.AddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));

            return new DryvBuilder(services)
                .AddTranslator<ObjectTranslator>()
                .AddTranslator<DryvResultTranslator>()
                .AddTranslator<StringTranslator>()
                .AddTranslator<EnumerableTranslator>()
                .AddTranslator<RegexTranslator>();
        }
    }
}