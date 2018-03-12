using System;
using Dryv.MethodCallTranslation;
using Dryv.Translation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv.DependencyInjection
{
    public static class DryvServiceCollectionExtensions
    {
        public static IServiceCollection AddDryv(this IServiceCollection services) => services.AddDryv(null);

        public static IServiceCollection AddDryv(this IServiceCollection services, Action<DryvOptions> setupAction)
        {
            services.AddSingleton<IMethodCallTranslator, DefaultMethodCallTranslator>();
            services.AddSingleton<ITranslator, JavaScriptTranslator>();

            var options = new DryvOptions();
            setupAction?.Invoke(options);

            services.AddSingleton(Options.Create(options));
            return services;
        }
    }
}