using System;
using System.Collections.Generic;
using System.Reflection;
using Dryv.DynamicControllers;
using Dryv.Extensions;
using Dryv.Translation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dryv
{
    public static class DryvApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDryv(this IApplicationBuilder app)
        {
            var endpointRouteBuilder = app.Properties["__EndpointRouteBuilder"] as IEndpointRouteBuilder;
            app.ApplicationServices.GetService<DryvEndpointRouteBuilderProvider>().RouteBuilder = endpointRouteBuilder;

            var dynamicControllerOptionsWrapper = app.ApplicationServices.GetService<IOptions<DryvDynamicControllerOptions>>();
            if (dynamicControllerOptionsWrapper != null)
            {
                SetupDynamicControllers(app, dynamicControllerOptionsWrapper);
            }

            var translatorProvider = app.ApplicationServices.GetService<ITranslatorProvider>();

            translatorProvider.MethodCallTranslators.AddRange(app.ApplicationServices.GetServices<IMethodCallTranslator>());
            translatorProvider.GenericTranslators.AddRange(app.ApplicationServices.GetServices<ICustomTranslator>());

            return app;
        }

        private static void DefaultEndpointMapping(IEndpointRouteBuilder builder, Type type, MethodInfo method)
        {
            var controller = type.Name.Replace("Controller", string.Empty);
            builder.MapControllerRoute(type.FullName, $"validation/{controller}/{method.Name}", new { controller, action = method.Name });
        }

        private static string DefaultTemplateMapping(string controller, string action, IDictionary<string, Type> arg3)
        {
            return $"validation/{controller.Replace("Controller", string.Empty)}/{action}";
        }

        private static void SetupDynamicControllers(IApplicationBuilder app, IOptions<DryvDynamicControllerOptions> dynamicControllerOptionsWrapper)
        {
            var dynamicControllerOptions = dynamicControllerOptionsWrapper.Value;
            var mvcOptions = app.ApplicationServices.GetService<IOptions<MvcOptions>>().Value;

            if (mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapEndpoint == null)
            {
                dynamicControllerOptions.MapEndpoint = DefaultEndpointMapping;
            }
            else if (!mvcOptions.EnableEndpointRouting && dynamicControllerOptions.MapTemplate == null)
            {
                dynamicControllerOptions.MapTemplate = DefaultTemplateMapping;
            }
        }
    }
}