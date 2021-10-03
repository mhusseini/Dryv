using System;
using System.Linq;
using Dryv.AspNetCore.DynamicControllers.Endpoints;
using Dryv.AspNetCore.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dryv.AspNetCore
{
    public static class DryvApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDryv(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices.GetService<DryvEndpointRouteBuilderProvider>();
            if (provider == null)
            {
                throw new InvalidOperationException($"Please call {nameof(DryvMvcBuilderExtensions)}.{nameof(DryvMvcBuilderExtensions.AddDryv)} before using {nameof(UseDryv)}.");
            }

            var endpointRouteBuilder = app.Properties.Values.OfType<IEndpointRouteBuilder>().FirstOrDefault();
            provider.RouteBuilder = endpointRouteBuilder;

            foreach (var initializer in app.ApplicationServices.GetServices<DryvMvcInitializer>())
            {
                initializer.Run(app.ApplicationServices);
            }

            return app;
        }
    }
}