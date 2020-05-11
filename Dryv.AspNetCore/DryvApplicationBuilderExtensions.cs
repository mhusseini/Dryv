using Dryv.AspNetCore.DynamicControllers;
using Dryv.AspNetCore.Internal;
using Dryv.Extensions;
using Dryv.Translation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class DryvApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDryv(this IApplicationBuilder app)
        {
            var endpointRouteBuilder = app.Properties["__EndpointRouteBuilder"] as IEndpointRouteBuilder;
            app.ApplicationServices.GetService<DryvEndpointRouteBuilderProvider>().RouteBuilder = endpointRouteBuilder;
            var translatorProvider = app.ApplicationServices.GetService<ITranslatorProvider>();

            translatorProvider.MethodCallTranslators.AddRange(app.ApplicationServices.GetServices<IMethodCallTranslator>());
            translatorProvider.GenericTranslators.AddRange(app.ApplicationServices.GetServices<ICustomTranslator>());

            foreach (var initializer in app.ApplicationServices.GetServices<DryvMvcInitializer>())
            {
                initializer.Run(app.ApplicationServices);
            }

            return app;
        }
    }
}