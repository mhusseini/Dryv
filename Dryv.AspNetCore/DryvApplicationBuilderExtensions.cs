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
            var endpointRouteBuilder = app.Properties.Values.OfType<IEndpointRouteBuilder>().FirstOrDefault();
            app.ApplicationServices.GetService<DryvEndpointRouteBuilderProvider>().RouteBuilder = endpointRouteBuilder;

            foreach (var initializer in app.ApplicationServices.GetServices<DryvMvcInitializer>())
            {
                initializer.Run(app.ApplicationServices);
            }

            return app;
        }
    }
}