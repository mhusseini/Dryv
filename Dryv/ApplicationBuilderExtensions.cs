using System;
using Dryv.DependencyInjection;
using Dryv.Translation;
using Dryv.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddGenericTranslator<T>(this IDryvBuilder dryvBuilder)
            where T : class, ICustomTranslator
            => dryvBuilder.Services.AddSingleton<ICustomTranslator, T>();

        public static IServiceCollection AddGenericTranslator(this IDryvBuilder dryvBuilder, Type type)
            => AddSingleton(dryvBuilder.Services, typeof(ICustomTranslator), type);

        public static IServiceCollection AddMethodCallTranslator<T>(this IDryvBuilder dryvBuilder)
            where T : class, IMethodCallTranslator
            => dryvBuilder.Services.AddSingleton<IMethodCallTranslator, T>();

        public static IServiceCollection AddMethodCallTranslator(this IDryvBuilder dryvBuilder, Type type)
            => AddSingleton(dryvBuilder.Services, typeof(IMethodCallTranslator), type);

        public static IApplicationBuilder UseDryv(this IApplicationBuilder app)
        {
            var translatorProvider = app.ApplicationServices.GetService<ITranslatorProvider>();

            translatorProvider.MethodCallTranslators.AddRange(app.ApplicationServices.GetServices<IMethodCallTranslator>());
            translatorProvider.GenericTranslators.AddRange(app.ApplicationServices.GetServices<ICustomTranslator>());

            return app;
        }

        private static IServiceCollection AddSingleton(IServiceCollection services, Type iface, Type type)
        {
            if (!iface.IsAssignableFrom(type))
            {
                throw new ArgumentNullException(nameof(type), $"The type {type.FullName} does not implement {iface.Name}.");
            }

            return services.AddSingleton(iface, type);
        }
    }
}