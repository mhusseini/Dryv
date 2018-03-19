using System;
using Dryv.Configuration;
using Dryv.DependencyInjection;
using Dryv.MethodCallTranslation;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddSingleton<ITranslator, JavaScriptTranslator>();
            services.AddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));

            return new DryvBuilder(services)
                .AddTranslator<DryvResultTranslator>()
                .AddTranslator<StringTranslator>()
                .AddTranslator<RegexTranslator>();
        }
    }
}