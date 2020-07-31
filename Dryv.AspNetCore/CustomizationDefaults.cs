using Dryv.AspNetCore.DynamicControllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Dryv.AspNetCore
{
    public static class CustomizationDefaults
    {
        private static int controllerCount;

        public static void DefaultEndpoint(DryvControllerGenerationContext context, IEndpointRouteBuilder builder) =>
            builder.MapControllerRoute(
                context.ControllerFullName,
                $"validation/{context.Controller}/{context.Action}",
                new { controller = context.Controller, action = context.Action });

        public static DryvDynamicControllerMethods DefaultHttpMethod(DryvControllerGenerationContext context) => DryvDynamicControllerMethods.Post;

        public static string DefaultRoute(DryvControllerGenerationContext context) => $"_v/c{++controllerCount}";
    }
}