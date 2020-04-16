using Dryv.Extensions;
using Dryv.Translation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDryv(this IApplicationBuilder app)
        {
            var translatorProvider = app.ApplicationServices.GetService<ITranslatorProvider>();

            translatorProvider.MethodCallTranslators.AddRange(app.ApplicationServices.GetServices<IMethodCallTranslator>());
            translatorProvider.GenericTranslators.AddRange(app.ApplicationServices.GetServices<ICustomTranslator>());

            return app;
        }
    }
}