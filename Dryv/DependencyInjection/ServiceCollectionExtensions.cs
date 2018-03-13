using System;
using Dryv.MethodCallTranslation;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            services.AddSingleton<IMethodCallTranslator, DefaultMethodCallTranslator>();
            services.AddSingleton<ITranslator, JavaScriptTranslator>();
            services.AddSingleton<IMethodCallTranslator, StringMethodCallTranslator>();
            services.AddSingleton<IMethodCallTranslator, RegexMethodCallTranslator>();
            services.AddSingleton<ITranslatorProvider, TranslatorProvider>();
            services.AddSingleton(Options.Create(options));

            return new DryvBuilder(services);
        }
    }
}