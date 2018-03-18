using Dryv.MethodCallTranslation;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Dryv.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IDryvBuilder AddDryv(this IServiceCollection services)
            => services.AddDryv(null);

        public static IDryvBuilder AddDryv(this IServiceCollection services, Action<DryvOptions> setupAction)
        {
            var options = new DryvOptions();
            setupAction?.Invoke(options);

            services.AddSingleton<ITranslator, JavaScriptTranslator>();
            services.AddTransient<IMethodCallTranslator, StringTranslator>();
            services.AddTransient<IMethodCallTranslator, RegexTranslator>();
            services.AddTransient<IGenericTranslator, RegexTranslator>();
            services.AddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton<IMethodCallTranslator, DefaultTranslator>();
            services.AddSingleton(Options.Create(options));

            return new DryvBuilder(services);
        }
    }
}