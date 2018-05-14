using System;
using Dryv.Configuration;
using Dryv.Mvc;
using Dryv.Translation;
using Dryv.Translation.Translators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public static class ServiceCollectionExtensions
    {
        public static IDryvBuilder AddDryv(this IServiceCollection services)
        {
            return services.AddDryv(null);
        }

        public static IDryvBuilder AddDryv(this IServiceCollection services, Action<DryvOptions> setupAction)
        {
            var options = new DryvOptions();

            setupAction?.Invoke(options);

            services.TryAddSingleton<IDryvClientModelValidator, DryvClientModelValidator>();
            services.AddSingleton<ITranslator, JavaScriptTranslator>();
            services.AddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));
            services.AddScoped<IModelProvider, ModelProvider>();
            services.AddSingleton<IObjectModelValidator, ObjectModelValidator>(s => new ObjectModelValidator(
                s.GetRequiredService<IModelMetadataProvider>(),
                s.GetRequiredService<IOptions<MvcOptions>>().Value.ModelValidatorProviders));

            return new DryvBuilder(services)
                .AddTranslator<ObjectTranslator>()
                .AddTranslator<DryvResultTranslator>()
                .AddTranslator<StringTranslator>()
                .AddTranslator<EnumerableTranslator>()
                .AddTranslator<RegexTranslator>();
        }
    }
}